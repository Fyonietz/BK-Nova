using Dapper;
using BKNova.Models;


namespace BKNova.Services
{

    public class AumServices
    {
        private readonly Database db;
        public AumServices(Database _db) => db = _db;

        public async Task<bool> SubmitAUM(SubmitAUM data)
        {
            using var conn = db.connect();
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                // 1. Resolve Id_User -> Siswa.Id
                var idSiswa = await conn.QueryFirstOrDefaultAsync<int?>(
                    "SELECT Id FROM Siswa WHERE Id_User = @Id_User", new { data.Id_User }, transaction);

                if (idSiswa == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // 2. Cek belum pernah submit di tahun ajaran ini (cegah double submit)
                var existing = await conn.QueryFirstOrDefaultAsync<int?>(
                    "SELECT Id FROM Status_Submit_AUM WHERE Id_Siswa = @idSiswa AND Id_Tahun_Ajaran = @Id_Tahun_Ajaran",
                    new { idSiswa, data.Id_Tahun_Ajaran }, transaction);

                if (existing != null)
                {
                    await transaction.RollbackAsync();
                    return false; // sudah submit sebelumnya
                }

                // 3. Insert semua jawaban (hanya soal yang ditandai "bermasalah")
                if (data.Id_Soal_Masalah_Terpilih.Count > 0)
                {
                    var hasilRows = data.Id_Soal_Masalah_Terpilih.Select(idSoal => new
                    {
                        Id_Siswa = idSiswa,
                        Id_Soal_Masalah = idSoal,
                        data.Id_Tahun_Ajaran
                    });

                    string sql_hasil = @"INSERT INTO Hasil_AUM(Id_Siswa, Id_Soal_Masalah, Id_Tahun_Ajaran)
                                  VALUES(@Id_Siswa, @Id_Soal_Masalah, @Id_Tahun_Ajaran)";
                    await conn.ExecuteAsync(sql_hasil, hasilRows, transaction);
                }

                // 4. Tandai siswa sudah submit
                string sql_status = @"INSERT INTO Status_Submit_AUM(Id_Siswa, Id_Tahun_Ajaran)
                               VALUES(@idSiswa, @Id_Tahun_Ajaran)";
                await conn.ExecuteAsync(sql_status, new { idSiswa, data.Id_Tahun_Ajaran }, transaction);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> HasSubmitted(int idUser)
        {
            using var conn = db.connect();
            string sql = @"SELECT COUNT(1) FROM Status_Submit_AUM ss
                    JOIN Siswa s ON s.Id = ss.Id_Siswa
                    WHERE s.Id_User = @idUser";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { idUser });
            return count > 0;
        }

        public async Task<HasilAUMGrouped?> GetHasilBySiswa(int idSiswa)
        {
            using var conn = db.connect();
            string sql = @"
                  SELECT 
                    h.Id AS Id,
                    s.Id AS IdSiswa,
                    h.Creted_At AS WaktuMengisi,
                    s.NIS,
                    s.NISN,
                    u.Nama AS Nama,
                    k.Nama AS Kelas,
                    k.Tingkat AS Tingkat,
                    b.Kode AS Kode_Bidang,
                    b.Nama AS Nama_Bidang,
                    sm.Pertanyaan AS Pilihan
                  FROM Hasil_AUM h
                  JOIN Siswa s ON s.Id = h.Id_Siswa
                  JOIN User u ON u.Id = s.Id_User
                  JOIN Kelas k ON k.Id = s.Id_Kelas
                  JOIN Soal_Masalah sm ON sm.Id = h.Id_Soal_Masalah
                  JOIN Bidang_Masalah b ON b.Id = sm.Id_Bidang_Masalah
                  WHERE s.Id = @idSiswa";

            var rows = (await conn.QueryAsync<HasilAUMFlat>(sql, new { idSiswa })).ToList();
            if (rows.Count == 0) return null;

            var grouped = new HasilAUMGrouped
            {
                IdSiswa = rows[0].IdSiswa,
                Nama = rows[0].Nama,
                Kelas = rows[0].Kelas,
                Tingkat = rows[0].Tingkat,
                NIS = rows[0].NIS,
                NISN = rows[0].NISN,
                WaktuMengisi = rows[0].WaktuMengisi,
                Bidang = rows
                    .GroupBy(r => new { r.Kode_Bidang, r.Nama_Bidang })
                    .Select(g => new BidangGrouped
                    {
                        Kode_Bidang = g.Key.Kode_Bidang,
                        Nama_Bidang = g.Key.Nama_Bidang,
                        Pilihan = g.Select(x => x.Pilihan).ToList()
                    })
                    .ToList()
            };

            return grouped;
        }
    }//Class
}//Namespace
