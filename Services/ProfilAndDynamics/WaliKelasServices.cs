using BKNova.Models;
using Dapper;

namespace BKNova.Services
{
    public class WaliKelasServices
    {
        private readonly Database db;
        public WaliKelasServices(Database _db) => db = _db;

        public async Task<bool> Register(RegisterWaliKelas data)
        {
            using var conn = db.connect();
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                // 1. Insert User (guru) — sesuaikan Id_Role guru kamu
                var sql_user = @"INSERT INTO User(Nama, Password, Id_Role, Is_Active) 
                          VALUES(@Nama, @Password, @Id_Role, @Is_Active)";
                await conn.ExecuteAsync(sql_user, new
                {
                    Nama = data.user.Nama,
                    Password = data.user.Password,
                    Id_Role = 3, // <-- ganti sesuai Id_Role guru di tabel Role kamu
                    Is_Active = true
                }, transaction);

                // 2. Ambil Id_User yang baru dibuat
                var userId = await conn.ExecuteScalarAsync<ulong>(
                    "SELECT LAST_INSERT_ID()", transaction: transaction);

                // 3. Insert Wali_Kelas pakai Id_User tsb
                var sql_wali = @"INSERT INTO Wali_Kelas(Id_User, Id_Kelas, Id_Tahun_Ajaran)
                          VALUES(@Id_User, @Id_Kelas, @Id_Tahun_Ajaran)";
                int affected = await conn.ExecuteAsync(sql_wali, new
                {
                    Id_User = (int)userId,
                    data.wali_kelas.Id_Kelas,
                    data.wali_kelas.Id_Tahun_Ajaran
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

        public async Task<List<WaliKelasDTO>> GetAll()
        {
            using var conn = db.connect();
            string sql = @"SELECT 
              w.Id_User AS Id, 
              u.Nama as Nama,
              u.Refresh_Token,
              u.Refresh_Token_Expired,
              u.Created_At,
              u.Id as Id,
              k.Nama as Kelas,
              t.Nama AS Tahun_Ajaran
              FROM Wali_Kelas w
              JOIN User u ON u.Id = w.Id_User
              JOIN Kelas k ON k.Id = w.Id_Kelas
              JOIN Tahun_Ajaran t on t.Id = w.Id_Tahun_Ajaran";
            var result = await conn.QueryAsync<WaliKelasDTO>(sql);
            return result.ToList();
        }

        public async Task<WaliKelasDTO> GetById(int id)
        {
            using var conn = db.connect();
            string sql = @"SELECT 
              w.Id_User AS Id, 
              u.Nama as Nama,
              u.Refresh_Token,
              u.Refresh_Token_Expired,
              u.Created_At,
              u.Id as Id,
              k.Nama as Kelas,
              t.Nama AS Tahun_Ajaran
              FROM Wali_Kelas w
              JOIN User u ON u.Id = w.Id_User
              JOIN Kelas k ON k.Id = w.Id_Kelas
              JOIN Tahun_Ajaran t on t.Id = w.Id_Tahun_Ajaran WHERE Id_User = @Id";
            var result = await conn.QueryFirstOrDefaultAsync<WaliKelasDTO>(sql, new { Id = id });
            return result ?? new WaliKelasDTO();
        }

        public async Task<bool> Update(int id, UpdateWaliKelas data)
        {
            using var conn = db.connect();
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                string sql_user = @"
            UPDATE User 
            SET Nama = @Nama, 
                Password = COALESCE(@Password, Password), 
                Updated_At = @Updated_At 
            WHERE Id = @Id;";

                int user_status = await conn.ExecuteAsync(sql_user, new
                {
                    data.Nama,
                    Password = string.IsNullOrWhiteSpace(data.Password) ? null : data.Password,
                    Updated_At = DateTime.UtcNow,
                    Id = id
                }, transaction);

                string sql_wali = @"
            UPDATE Wali_Kelas 
            SET Id_Kelas = @Id_Kelas, 
                Id_Tahun_Ajaran = @Id_Tahun_Ajaran 
            WHERE Id_User = @Id;";

                int wali_status = await conn.ExecuteAsync(sql_wali, new
                {
                    data.Id_Kelas,
                    data.Id_Tahun_Ajaran,
                    Id = id
                }, transaction);

                if (user_status >= 0 && wali_status >= 0)
                {
                    await transaction.CommitAsync();
                    return true;
                }

                await transaction.RollbackAsync();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Error: {ex.Message}");
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> Delete(int id){
          using var conn = db.connect();
          await conn.OpenAsync();
          using(var transaction=conn.BeginTransaction()){
            try{
               string sql_wali = @"DELETE FROM Wali_Kelas WHERE Id_User=@Id";
               int wali_status = await conn.ExecuteAsync(sql_wali,new {Id = id},transaction);

               string sql_user = @"DELETE FROM User WHERE Id=@Id";
               int user_status = await conn.ExecuteAsync(sql_user,new {Id = id},transaction);

               if(wali_status > 0 && user_status > 0){
                 await transaction.CommitAsync();
                 return true;
               }

               await transaction.RollbackAsync();
               return false;
            }catch(Exception ex){
              Console.WriteLine($"Update Error: {ex.Message}");
              await transaction.RollbackAsync();
              return false;

            }

          }
        }
    }
}
