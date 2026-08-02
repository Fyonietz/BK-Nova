using BKNova.Models;
using Dapper;
namespace BKNova.Services
{
    public class TahunAjaranServices
    {
        private readonly Database db;
        public TahunAjaranServices(Database _db) => db = _db;

        public async Task<bool> Create(Tahun_Ajaran data)
        {
            using var conn = db.connect();

            var sql = @"
        INSERT INTO Tahun_Ajaran (Nama, Semester, Is_Active)
        VALUES (@Nama, @Semester, @Is_Active)";

            var rows = await conn.ExecuteAsync(sql, new { Nama = data.Nama, Semester = data.Semester.ToString(), Is_Active = data.Is_Active });

            return rows > 0;
        }
        public async Task<List<Tahun_AjaranDTO>> GetAll()
        {
            using var conn = db.connect();

            var sql = @"SELECT * FROM Tahun_Ajaran";

            var result = await conn.QueryAsync<Tahun_AjaranDTO>(sql);
            return result.ToList();
        }
        public async Task<Tahun_AjaranDTO?> Get(int Id)
        {
            using var conn = db.connect();

            var sql = @"SELECT * FROM Tahun_Ajaran WHERE Id=@id";

            var result = await conn.QueryFirstOrDefaultAsync<Tahun_AjaranDTO>(sql, new { Id = Id });
            return result;
        }

        public async Task<bool> Delete(int Id)
        {
            using var conn = db.connect();

            var sql = @"DELETE FROM Tahun_Ajaran WHERE Id=@id";

            return await conn.ExecuteAsync(sql, new { Id = Id }) > 0;

        }
        public async Task<bool> Update(int Id, Tahun_Ajaran data)
        {
            using var conn = db.connect();

            var sql = @"UPDATE Tahun_Ajaran SET Nama=@Nama,Semester=@Semester,Is_Active=@Is_Active WHERE Id=@Id";

            return await conn.ExecuteAsync(sql, new { Id = Id, Nama = data.Nama, Semester = data.Semester.ToString(), Is_Active = data.Is_Active}) > 0;

        }

    }
}
