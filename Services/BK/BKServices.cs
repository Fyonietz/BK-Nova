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

        public async Task<PaginatedResponse<BKDTO>> GetAllPaginated(int page, int pageSize)
        {
            var (safePage, safePageSize) = PaginationHelper.Normalize(page, pageSize);
            int offset = (safePage - 1) * safePageSize;

            using var conn = db.connect();
            string countSql = @"SELECT COUNT(*) FROM User WHERE Id_Role = 2";
            string sql = @"SELECT u.Id, u.Nama, r.Nama AS Role
                           FROM User u
                           JOIN Roles r ON r.Id = u.Id_Role
                           WHERE Id_Role = 2
                           ORDER BY u.Id
                           LIMIT @PageSize OFFSET @Offset";

            var totalItems = await conn.ExecuteScalarAsync<int>(countSql);
            var rows = (await conn.QueryAsync<BKDTO>(sql, new { PageSize = safePageSize, Offset = offset })).ToList();

            return new PaginatedResponse<BKDTO>
            {
                Page = safePage,
                PageSize = safePageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / safePageSize),
                HasNextPage = safePage < (totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / safePageSize)),
                HasPreviousPage = safePage > 1,
                Data = rows
            };
        }
        public async Task<bool> AssignTugas(TugasBK data)
        {
            using var conn = db.connect();
            string sql = @"INSERT INTO Tugas_BK(Id_User_BK, Id_Kelas, Id_Tahun_Ajaran, Is_Active)
                    VALUES(@Id_User_BK, @Id_Kelas, @Id_Tahun_Ajaran, @Is_Active)";
            int affected = await conn.ExecuteAsync(sql, data);
            return affected > 0;
        }

        public async Task<List<TugasBKDTO>> GetAllTugas()
        {
            using var conn = db.connect();
            string sql = @"SELECT 
        tb.Id, tb.Id_User_BK, u.Nama AS Nama_BK,
        tb.Id_Kelas, k.Nama AS Nama_Kelas, k.Tingkat,
        tb.Id_Tahun_Ajaran, ta.Nama AS TahunAjaran,
        tb.Is_Active, tb.Assigned_At
      FROM Tugas_BK tb
      JOIN User u ON u.Id = tb.Id_User_BK
      JOIN Kelas k ON k.Id = tb.Id_Kelas
      JOIN Tahun_Ajaran ta ON ta.Id = tb.Id_Tahun_Ajaran";
            var result = await conn.QueryAsync<TugasBKDTO>(sql);
            return result.ToList();
        }

        public async Task<PaginatedResponse<TugasBKDTO>> GetAllTugasPaginated(int page, int pageSize)
        {
            var (safePage, safePageSize) = PaginationHelper.Normalize(page, pageSize);
            int offset = (safePage - 1) * safePageSize;

            using var conn = db.connect();
            string countSql = @"SELECT COUNT(*) FROM Tugas_BK";
            string sql = @"SELECT 
        tb.Id, tb.Id_User_BK, u.Nama AS Nama_BK,
        tb.Id_Kelas, k.Nama AS Nama_Kelas, k.Tingkat,
        tb.Id_Tahun_Ajaran, ta.Nama AS TahunAjaran,
        tb.Is_Active, tb.Assigned_At
      FROM Tugas_BK tb
      JOIN User u ON u.Id = tb.Id_User_BK
      JOIN Kelas k ON k.Id = tb.Id_Kelas
      JOIN Tahun_Ajaran ta ON ta.Id = tb.Id_Tahun_Ajaran
      ORDER BY tb.Id
      LIMIT @PageSize OFFSET @Offset";

            var totalItems = await conn.ExecuteScalarAsync<int>(countSql);
            var rows = (await conn.QueryAsync<TugasBKDTO>(sql, new { PageSize = safePageSize, Offset = offset })).ToList();
            int totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / safePageSize);

            return new PaginatedResponse<TugasBKDTO>
            {
                Page = safePage,
                PageSize = safePageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasNextPage = safePage < totalPages,
                HasPreviousPage = safePage > 1,
                Data = rows
            };
        }

        public async Task<List<TugasBKDTO>> GetTugasByBK(int idUserBK)
        {
            using var conn = db.connect();
            string sql = @"SELECT 
        tb.Id, tb.Id_User_BK, u.Nama AS Nama_BK,
        tb.Id_Kelas, k.Nama AS Nama_Kelas, k.Tingkat,
        tb.Id_Tahun_Ajaran, ta.Nama AS TahunAjaran,
        tb.Is_Active, tb.Assigned_At
      FROM Tugas_BK tb
      JOIN User u ON u.Id = tb.Id_User_BK
      JOIN Kelas k ON k.Id = tb.Id_Kelas
      JOIN Tahun_Ajaran ta ON ta.Id = tb.Id_Tahun_Ajaran
      WHERE tb.Id_User_BK = @idUserBK";
            var result = await conn.QueryAsync<TugasBKDTO>(sql, new { idUserBK });
            return result.ToList();
        }

        public async Task<PaginatedResponse<TugasBKDTO>> GetTugasByBKPaginated(int idUserBK, int page, int pageSize)
        {
            var (safePage, safePageSize) = PaginationHelper.Normalize(page, pageSize);
            int offset = (safePage - 1) * safePageSize;

            using var conn = db.connect();
            string countSql = @"SELECT COUNT(*) FROM Tugas_BK WHERE Id_User_BK = @IdUserBK";
            string sql = @"SELECT 
        tb.Id, tb.Id_User_BK, u.Nama AS Nama_BK,
        tb.Id_Kelas, k.Nama AS Nama_Kelas, k.Tingkat,
        tb.Id_Tahun_Ajaran, ta.Nama AS TahunAjaran,
        tb.Is_Active, tb.Assigned_At
      FROM Tugas_BK tb
      JOIN User u ON u.Id = tb.Id_User_BK
      JOIN Kelas k ON k.Id = tb.Id_Kelas
      JOIN Tahun_Ajaran ta ON ta.Id = tb.Id_Tahun_Ajaran
      WHERE tb.Id_User_BK = @IdUserBK
      ORDER BY tb.Id
      LIMIT @PageSize OFFSET @Offset";

            var totalItems = await conn.ExecuteScalarAsync<int>(countSql, new { IdUserBK = idUserBK });
            var rows = (await conn.QueryAsync<TugasBKDTO>(sql, new { IdUserBK = idUserBK, PageSize = safePageSize, Offset = offset })).ToList();
            int totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / safePageSize);

            return new PaginatedResponse<TugasBKDTO>
            {
                Page = safePage,
                PageSize = safePageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasNextPage = safePage < totalPages,
                HasPreviousPage = safePage > 1,
                Data = rows
            };
        }

        public async Task<bool> UpdateTugas(int id, UpdateTugasBK data)
        {
            using var conn = db.connect();
            string sql = @"UPDATE Tugas_BK 
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

        public async Task<bool> DeleteTugas(int id)
        {
            using var conn = db.connect();
            string sql = @"DELETE FROM Tugas_BK WHERE Id = @Id";
            int affected = await conn.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

    }
}
