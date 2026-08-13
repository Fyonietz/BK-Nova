using Dapper;
using BKNova.Models;
namespace BKNova.Services
{
    public class AuthServices
    {
        private readonly Database db;
        public AuthServices(Database _db) => db = _db;

        public async Task<bool> RegisterAdmin(AdminRegister data)
        {
            using var conn = db.connect();
            string sql = @"INSERT INTO User(Nama,Id_Role,Password,Is_Active) VALUES(@Nama,@Id_Role,@Password,@Is_Active)";
            return await conn.ExecuteAsync(sql, new
            {
                Nama = data.Nama,
                Id_Role = 1,
                Password = data.Password,
                Is_Active = true
            }) > 0;
        }

        public async Task<bool> AdminIsRegistered()
        {
            using var conn = db.connect();
            var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM User");
            return count > 0;
        }

        public async Task<User?> Login(Login data)
        {
            using var conn = db.connect();
            var sql = @"
        SELECT
            u.Id,
            u.Nama,
            u.Password,
            u.Is_Active,
            r.Nama AS Role
        FROM User u
        JOIN Roles r ON u.Id_Role = r.Id
        WHERE u.Nama = @Nama;";
            return await conn.QueryFirstOrDefaultAsync<User>(sql, data);
        }
        public async Task UpdateRefreshToken(string RefreshToken, DateTime Expired, int user_id)
        {
            using var conn = db.connect();
            var sql = @"UPDATE User SET Refresh_Token = @RefreshToken,Refresh_Token_Expired=@Expired WHERE Id=@user_id";
            await conn.ExecuteAsync(sql, new { RefreshToken = RefreshToken, Expired = Expired, user_id = user_id });
        }
        public async Task<User?> RefreshTokenService(RefreshRequest req)
        {
            using var conn = db.connect();
            var sql = @"
        SELECT
            u.Id,
            u.Nama,
            u.Password,
            u.Is_Active,
            r.Nama AS Role
        FROM User u
        JOIN Roles r ON u.Id_Role = r.Id
        WHERE u.Refresh_Token = @refreshToken;";
            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { refreshToken = req.Refresh_Token });
        }

        public async Task<User?> GetMe(int userId)
        {
            using var conn = db.connect();

            var sql = @"
              SELECT
                  u.Id,
                  u.Nama,
                  u.Id_Role,
                  u.Is_Active,
                  r.Nama AS Role
              FROM User u
              JOIN Roles r ON u.Id_Role = r.Id
              WHERE u.Id = @Id;";

            return await conn.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Id = userId }
            );
        }

        public async Task<SiswaProfile?> GetSiswaProfile(int userId)
        {
            using var conn = db.connect();

            var sql = @"
                  SELECT
                  s.Id,
                  s.Id_User,
                  s.NISN,
                  s.NIS,
                  s.Jenis_Kelamin,
                  s.Tempat_Tanggal_Lahir,
                  k.Nama as Kelas,
                  k.Tingkat as Tingkat,
                  j.Nama AS Jurusan
              FROM Siswa s
              JOIN Kelas k ON k.Id = s.Id_Kelas
              JOIN Jurusan j on j.Id = k.Id_Jurusan
              WHERE s.Id_User = @UserId"
              ;

            return await conn.QueryFirstOrDefaultAsync<SiswaProfile>(
                sql,
                new { UserId = userId }
            );
        }
        public async Task<WaliKelasProfile?> GetWaliKelasProfile(int userId)
        {
            using var conn = db.connect();

            var sql = @"
                    SELECT
                    w.Id,
                    w.Id_User,
                    k.Nama AS Kelas,
                    k.Tingkat AS Tingkat,
                    t.Nama AS TahunAjaran
                    
                FROM Wali_Kelas w
                JOIN Kelas k ON w.Id_Kelas = k.Id
                JOIN Tahun_Ajaran t on w.Id_Kelas = t.Id
                WHERE w.Id_User = @UserId";

            return await conn.QueryFirstOrDefaultAsync<WaliKelasProfile>(
                sql,
                new { UserId = userId }
            );
        }
    }//Class
}//Namespace
