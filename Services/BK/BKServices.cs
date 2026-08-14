using Dapper;
using BKNova.Models;

namespace BKNova.Services
{
    public class BKServices
    {
        private readonly Database db;
        public BKServices(Database _db)
        {
            db = _db;
        }

        public async Task<bool> Create(UserCreate user)
        {
            using var conn = db.connect();
            string sql = @"
        INSERT INTO User(Nama,Password,Id_Role) VALUES(@Nama,@Password,@Id_Role)
        ";
            return await conn.ExecuteAsync(sql, new { Nama = user.Nama, Password = user.Password, Id_Role = 2 }) > 0;
        }

        public async Task<List<BKDTO>> GetAll()
        {
            using var conn = db.connect();
            string sql = @"SELECT u.Id,u.Nama,r.Nama AS Role FROM User u JOIN Roles r ON r.Id = u.Id_Role WHERE Id_Role = 2";
            var res = await conn.QueryAsync<BKDTO>(sql);
            return res.ToList();
        }


    }
}
