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

        public async Task<bool> HasSubmitted(int idUser, int idTahunAjaran)
        {
            using var conn = db.connect();
            string sql = @"SELECT COUNT(1) FROM Status_Submit_AUM ss
                    JOIN Siswa s ON s.Id = ss.Id_Siswa
                    WHERE s.Id_User = @idUser AND ss.Id_Tahun_Ajaran = @idTahunAjaran";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { idUser, idTahunAjaran });
            return count > 0;
        }
    }//Class
}//Namespace
