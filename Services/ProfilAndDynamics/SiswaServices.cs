using BKNova.Models;
using Dapper;
namespace BKNova.Services
{
    public class SiswaServices
    {
        private readonly Database db;

        public SiswaServices(Database _db) => db = _db;

        public async Task<bool> Register(RegisterSiswa data)
        {
            using (var conn = db.connect())
            {
                await conn.OpenAsync(); // Mandatory for transactions

                using (var transaction = await conn.BeginTransactionAsync())
                {

                    var JenisKelaminConverted = data.siswa.Kelamin switch
                    {
                        JenisKelamin.Laki => "Laki-Laki",
                        JenisKelamin.Perempuan => "Perempuan",
                        _ => throw new ArgumentException("Invalid gender value")
                    };

                    try
                    {
                        // 1. Execute INSERT only (returns rows affected, not ID)
                        var sql_user = @"INSERT INTO User(Nama, Password, Id_Role, Is_Active) 
                             VALUES(@Nama, @Password, @Id_Role, @Is_Active)";

                        await conn.ExecuteAsync(sql_user, new
                        {
                            Nama = data.user.Nama,
                            Password = data.user.Password,
                            Id_Role = 4,
                            Is_Active = true
                        }, transaction);

                        // 2. Explicitly fetch the ID in a separate command
                        // Use 'ulong' for MariaDB/MySQL as LAST_INSERT_ID() returns BIGINT UNSIGNED
                        var userId = await conn.ExecuteScalarAsync<ulong>(
                            "SELECT LAST_INSERT_ID()",
                            transaction: transaction
                        );

                        // 3. Insert Siswa using the retrieved ID
                        var sql_siswa = @"INSERT INTO Siswa(Id_User, NISN, NIS, Jenis_Kelamin, Tempat_Tanggal_Lahir, Id_Kelas) 
                              VALUES(@Id_User, @NISN, @NIS, @Jenis_Kelamin, @TempatLahir, @Id_Kelas)";

                        int affected_row = await conn.ExecuteAsync(sql_siswa, new
                        {
                            Id_User = (int)userId, // Cast ulong to int if your Id is INT
                            NISN = data.siswa.NISN,
                            NIS = data.siswa.NIS,
                            Id_Kelas = data.siswa.Id_Kelas,
                            Jenis_Kelamin = JenisKelaminConverted,
                            TempatLahir = data.siswa.Tempat_Tanggal_Lahir
                        }, transaction);

                        await transaction.CommitAsync();
                        return affected_row > 0;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        // Log ex.Message for debugging the exact SQL error
                        throw; // Re-throw to let your API controller catch it
                    }
                }

            }
        }
        public async Task<List<SiswaDTO>> GetAll()
        {
            using var conn = db.connect();
            string sql = @"SELECT 
              s.NIS,
              s.NISN,
              s.Jenis_Kelamin AS Kelamin,
              s.Tempat_Tanggal_Lahir,
              u.Nama as Nama,
              u.Refresh_Token,
              u.Refresh_Token_Expired,
              u.Created_At,
              u.Id as Id,
              k.Nama as Kelas,
              k.Tingkat as Tingkat
              FROM Siswa s 
              JOIN User u ON u.id = s.Id_User
              JOIN Kelas k ON k.Id = s.Id_Kelas";
            var result = await conn.QueryAsync<SiswaDTO>(sql);
            return result.ToList();
        }
        public async Task<SiswaDTO> GetById(int Id)
        {
            using var conn = db.connect();
            string sql = @"SELECT 
              s.NIS,
              s.NISN,
              s.Jenis_Kelamin AS Kelamin,
              s.Tempat_Tanggal_Lahir,
              u.Nama as Nama,
              u.Refresh_Token,
              u.Refresh_Token_Expired,
              u.Created_At,
              u.Id as Id,
              k.Nama as Kelas
              FROM Siswa s 
              JOIN User u ON u.id = s.Id_User
              JOIN Kelas k ON k.Id = s.Id_Kelas WHERE s.Id_User = @Id";
            var result = await conn.QueryFirstOrDefaultAsync<SiswaDTO>(sql, new { Id = Id });
            return result ?? new SiswaDTO();
        }
        public async Task<bool> Update(int Id, UpdateSiswa data)
        {
            using var conn = db.connect();
            await conn.OpenAsync();

            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                var JenisKelaminConverted = data.Kelamin switch
                {
                    JenisKelamin.Laki => "Laki-Laki",
                    JenisKelamin.Perempuan => "Perempuan",
                    _ => throw new ArgumentException("Invalid gender value")
                };

                // COALESCE keeps the old password if data.Password is null
                string sql_user = @"
            UPDATE User 
            SET Nama = @Nama, 
                Password = COALESCE(@Password, Password), 
                Updated_At = @Updated_At 
            WHERE Id = @Id;";

                int user_status = await conn.ExecuteAsync(sql_user, new
                {
                    Nama = data.Nama,
                    Password = string.IsNullOrWhiteSpace(data.Password) ? null : data.Password,
                    Updated_At = DateTime.UtcNow,
                    Id = Id
                }, transaction);

                string sql_siswa = @"
            UPDATE Siswa 
            SET NISN = @Nisn, 
                NIS = @Nis, 
                Jenis_Kelamin = @Kelamin, 
                Tempat_Tanggal_Lahir = @TempatTanggal, 
                Id_Kelas = @Kelas 
            WHERE Id_User = @Id;";

                int siswa_status = await conn.ExecuteAsync(sql_siswa, new
                {
                    Nisn = data.NISN,
                    Nis = data.NIS,
                    Kelamin = JenisKelaminConverted,
                    TempatTanggal = data.Tempat_Tanggal_Lahir,
                    Kelas = data.Id_Kelas,
                    Id = Id
                }, transaction);

                if (user_status >= 0 && siswa_status >= 0)
                {
                    await transaction.CommitAsync();
                    return true;
                }

                await transaction.RollbackAsync();
                return false;
            }
            catch (Exception ex)
            {
                // Print the real exception message to terminal for debugging
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
               string sql_siswa = @"DELETE FROM Siswa WHERE Id_User=@Id";
               int siswa_status = await conn.ExecuteAsync(sql_siswa,new {Id = id},transaction);

               string sql_user = @"DELETE FROM User WHERE Id=@Id";
               int user_status = await conn.ExecuteAsync(sql_user,new {Id = id},transaction);

               if(siswa_status > 0 && user_status > 0){
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









    }//Class
}//Namespace
