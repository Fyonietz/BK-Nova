
using BKNova.Models;
using Dapper;
namespace BKNova.Services
{
    public class KelasServices
    {
        private readonly Database db;
        public KelasServices(Database _db) => db = _db;

        public async Task<bool> Create(Kelas data)
        {
            using var conn = db.connect();

            var sql = @"
        INSERT INTO Kelas (Nama,Id_Jurusan)
        VALUES (@Nama, @Id_Jurusan)";

            var rows = await conn.ExecuteAsync(sql, new { Nama = data.Nama,Id_Jurusan=data.Id_Jurusan});

            return rows > 0;
        }
        public async Task<List<KelasDTO>> GetAll()
        {
            using var conn = db.connect();

            var sql = @"SELECT k.Id,k.Nama,k.Tingkat,j.Nama as Jurusan FROM Kelas k JOIN Jurusan j ON k.Id_Jurusan=j.Id";

            var result = await conn.QueryAsync<KelasDTO>(sql);
            return result.ToList();
        }
        public async Task<KelasDTO?> Get(int Id)
        {
            using var conn = db.connect();

            var sql = @"SELECT k.Id,k.Nama,j.Nama as Jurusan FROM Kelas k JOIN Jurusan j ON k.Id_Jurusan=j.Id WHERE k.Id = @Id";

            var result = await conn.QueryFirstOrDefaultAsync<KelasDTO>(sql, new { Id = Id });
            return result;
        }

        public async Task<bool> Delete(int Id)
        {
            using var conn = db.connect();

            var sql = @"DELETE FROM Kelas WHERE Id=@id";

            return await conn.ExecuteAsync(sql, new { Id = Id }) > 0;

        }
        public async Task<bool> Update(int Id, Kelas data)
        {
            using var conn = db.connect();

            var sql = @"UPDATE Kelas SET Nama=@Nama,Id_Jurusan=@Id_Jurusan WHERE Id=@Id";

            return await conn.ExecuteAsync(sql, new { Id = Id, Nama = data.Nama, Id_Jurusan=data.Id_Jurusan}) > 0;

        }

    }
}
