using BKNova.Models;
using Dapper;
namespace BKNova.Services
{
    public class JurusanServices
    {
        private readonly Database db;
        public JurusanServices(Database _db) => db = _db;

        public async Task<bool> Create(Jurusan data)
        {
            using var conn = db.connect();

            var sql = @"
        INSERT INTO Jurusan (Nama, Kode)
        VALUES (@Nama, @Kode)";

            var rows = await conn.ExecuteAsync(sql,data);

            return rows > 0;
        }
        public async Task<List<JurusanDTO>> GetAll()
        {
            using var conn = db.connect();

            var sql = @"SELECT * FROM Jurusan";

            var result = await conn.QueryAsync<JurusanDTO>(sql);
            return result.ToList();
        }
        public async Task<JurusanDTO?> Get(int Id)
        {
            using var conn = db.connect();

            var sql = @"SELECT * FROM Jurusan WHERE Id=@id";

            var result = await conn.QueryFirstOrDefaultAsync<JurusanDTO>(sql, new { Id = Id });
            return result;
        }

        public async Task<bool> Delete(int Id)
        {
            using var conn = db.connect();

            var sql = @"DELETE FROM Jurusan WHERE Id=@id";

            return await conn.ExecuteAsync(sql, new { Id = Id }) > 0;

        }
        public async Task<bool> Update(int Id, Jurusan data)
        {
            using var conn = db.connect();

            var sql = @"UPDATE Jurusan SET Nama=@Nama,Kode=@Kode WHERE Id=@Id";

            return await conn.ExecuteAsync(sql, new { Id = Id, Nama = data.Nama, Kode=data.Kode}) > 0;

        }

    }
}
