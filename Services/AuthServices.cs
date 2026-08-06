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
        public async Task<User?> RefreshTokenService(RefreshRequest req){
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
          return await conn.QueryFirstOrDefaultAsync<User>(sql,new {refreshToken = req.Refresh_Token});
        }
    }
}
