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

        // Ambil daftar Id_Kelas yang jadi tanggung jawab guru BK ini
        private async Task<List<int>> GetKelasTugasBK(int idUserGuru)
        {
            using var conn = db.connect();

            string sql = @"
        SELECT tb.Id_Kelas 
        FROM Tugas_BK tb
        JOIN BK b ON b.Id = tb.Id_BK
        WHERE b.Id_User = @idUserGuru AND tb.Is_Active = 1";

            var result = await conn.QueryAsync<int>(sql, new { idUserGuru });
            return result.ToList();
        }

        // Hasil AUM tapi hanya untuk kelas yang jadi tugas guru BK ini
        public async Task<List<HasilAUMGrouped>> GetHasilByBK(int idUserGuru)
        {
            var kelasIds = await GetKelasTugasBK(idUserGuru);
            if (kelasIds.Count == 0) return new List<HasilAUMGrouped>();

            using var conn = db.connect();
            string sql = @"
        SELECT 
          h.Id AS Id,
          s.Id AS IdSiswa,
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
        WHERE s.Id_Kelas IN @kelasIds";

            var rows = (await conn.QueryAsync<HasilAUMFlat>(sql, new { kelasIds })).ToList();

            return rows
                .GroupBy(r => r.IdSiswa)
                .Select(g => new HasilAUMGrouped
                {
                    IdSiswa = g.Key,
                    Nama = g.First().Nama,
                    Kelas = g.First().Kelas,
                    Tingkat = g.First().Tingkat,
                    Bidang = g.GroupBy(x => new { x.Kode_Bidang, x.Nama_Bidang })
                              .Select(bg => new BidangGrouped
                              {
                                  Kode_Bidang = bg.Key.Kode_Bidang,
                                  Nama_Bidang = bg.Key.Nama_Bidang,
                                  Pilihan = bg.Select(x => x.Pilihan).ToList()
                              }).ToList()
                }).ToList();
        }
    }//Class
}//Namespace
