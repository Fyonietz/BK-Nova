 using BKNova.Models;
using Dapper;

namespace BKNova.Services
{
    public class RiwayatKelasSiswaServices
    {
        private readonly Database db;
        public RiwayatKelasSiswaServices(Database _db) => db = _db;

        public async Task<bool> Register(RiwayatKelasSiswa data)
        {
            using var conn = db.connect();
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                var idSiswa = await conn.QueryFirstOrDefaultAsync<int?>(
                    "SELECT Id FROM Siswa WHERE Id_User = @Id_User", new { data.Id_User }, transaction);

                if (idSiswa == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                string sql = @"INSERT INTO Riwayat_Kelas_Siswa(Id_Siswa, Id_Kelas, Id_Tahun_Ajaran, Is_Active)
                                VALUES(@Id_Siswa, @Id_Kelas, @Id_Tahun_Ajaran, @Is_Active)";
                int affected = await conn.ExecuteAsync(sql, new
                {
                    Id_Siswa = idSiswa,
                    data.Id_Kelas,
                    data.Id_Tahun_Ajaran,
                    data.Is_Active
                }, transaction);

                await transaction.CommitAsync();
                return affected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<RiwayatKelasSiswaDTO>> GetAll()
        {
            using var conn = db.connect();
            string sql = @"SELECT 
                r.Id, u.Id AS Id_User, r.Id_Kelas, r.Id_Tahun_Ajaran, r.Is_Active,
                u.Nama AS Nama_Siswa, k.Nama AS Nama_Kelas
              FROM Riwayat_Kelas_Siswa r
              JOIN Siswa s ON s.Id = r.Id_Siswa
              JOIN User u ON u.Id = s.Id_User
              JOIN Kelas k ON k.Id = r.Id_Kelas";
            var result = await conn.QueryAsync<RiwayatKelasSiswaDTO>(sql);
            return result.ToList();
        }

        public async Task<RiwayatKelasSiswaDTO> GetById(int id)
        {
            using var conn = db.connect();
            string sql = @"SELECT 
                r.Id, u.Id AS Id_User, r.Id_Kelas, r.Id_Tahun_Ajaran, r.Is_Active,
                u.Nama AS Nama_Siswa, k.Nama AS Nama_Kelas
              FROM Riwayat_Kelas_Siswa r
              JOIN Siswa s ON s.Id = r.Id_Siswa
              JOIN User u ON u.Id = s.Id_User
              JOIN Kelas k ON k.Id = r.Id_Kelas
              WHERE r.Id = @Id";
            var result = await conn.QueryFirstOrDefaultAsync<RiwayatKelasSiswaDTO>(sql, new { Id = id });
            return result ?? new RiwayatKelasSiswaDTO();
        }

        public async Task<bool> Update(int id, UpdateRiwayatKelasSiswa data)
        {
            using var conn = db.connect();
            string sql = @"UPDATE Riwayat_Kelas_Siswa 
                            SET Id_Kelas = @Id_Kelas, Id_Tahun_Ajaran = @Id_Tahun_Ajaran, Is_Active = @Is_Active
                            WHERE Id = @Id";
            int affected = await conn.ExecuteAsync(sql, new
            {
                data.Id_Kelas,
                data.Id_Tahun_Ajaran,
                data.Is_Active,
                Id = id
            });
            return affected >= 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var conn = db.connect();
            string sql = @"DELETE FROM Riwayat_Kelas_Siswa WHERE Id = @Id";
            int affected = await conn.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        public async Task<int> PromoteKelas(PromoteKelas data)
        {
            using var conn = db.connect();
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                string sql_kelas_lama = @"SELECT Tingkat, Id_Jurusan FROM Kelas WHERE Id = @Id_Kelas_Lama";
                var kelasLama = await conn.QueryFirstOrDefaultAsync(sql_kelas_lama, new { data.Id_Kelas_Lama }, transaction);

                if (kelasLama == null)
                {
                    await transaction.RollbackAsync();
                    return -1;
                }

                string tingkatLama = kelasLama.Tingkat;
                int idJurusan = kelasLama.Id_Jurusan;

                string? tingkatBaru = tingkatLama switch
                {
                    "X" => "XI",
                    "XI" => "XII",
                    "XII" => null,
                    _ => null
                };

                if (tingkatBaru == null)
                {
                    await transaction.RollbackAsync();
                    return -2;
                }

                string sql_kelas_baru = @"SELECT Id FROM Kelas WHERE Tingkat = @tingkatBaru AND Id_Jurusan = @idJurusan LIMIT 1";
                var idKelasBaru = await conn.QueryFirstOrDefaultAsync<int?>(sql_kelas_baru, new { tingkatBaru, idJurusan }, transaction);

                if (idKelasBaru == null)
                {
                    await transaction.RollbackAsync();
                    return -3;
                }

                string sql_get_siswa = @"SELECT Id FROM Siswa WHERE Id_Kelas = @Id_Kelas_Lama";
                var siswaIds = (await conn.QueryAsync<int>(sql_get_siswa, new { data.Id_Kelas_Lama }, transaction)).ToList();

                if (siswaIds.Count == 0)
                {
                    await transaction.RollbackAsync();
                    return 0;
                }

                string sql_deactivate = @"
                    UPDATE Riwayat_Kelas_Siswa SET Is_Active = 0 
                    WHERE Id_Siswa IN @Ids AND Is_Active = 1";
                await conn.ExecuteAsync(sql_deactivate, new { Ids = siswaIds }, transaction);

                var riwayatBaru = siswaIds.Select(id => new
                {
                    Id_Siswa = id,
                    Id_Kelas_Baru = idKelasBaru,
                    data.Id_Tahun_Ajaran_Baru
                });
                string sql_insert = @"
                    INSERT INTO Riwayat_Kelas_Siswa(Id_Siswa, Id_Kelas, Id_Tahun_Ajaran, Is_Active)
                    VALUES(@Id_Siswa, @Id_Kelas_Baru, @Id_Tahun_Ajaran_Baru, 1)";
                await conn.ExecuteAsync(sql_insert, riwayatBaru, transaction);

                string sql_update_siswa = @"UPDATE Siswa SET Id_Kelas = @idKelasBaru WHERE Id_Kelas = @Id_Kelas_Lama";
                await conn.ExecuteAsync(sql_update_siswa, new { idKelasBaru, data.Id_Kelas_Lama }, transaction);

                await transaction.CommitAsync();
                return siswaIds.Count;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
