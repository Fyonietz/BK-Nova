
using BKNova.Models;
using Dapper;


namespace BKNova.Services
{
    public class TiketServices
    {

        private readonly Database db;
        private readonly FcmService fcm;
        public TiketServices(Database _db, FcmService fcmService) { db = _db; fcm = fcmService; }


        //Ajukan Tiket Ambil Id_User dari path /me atau Account di Android
        public async Task<(bool Success, int? Id_BK, int? Id_Siswa)> GetIdBKAndIdSiswa(int Id_User)
        {
            using var conn = db.connect();
            string Sql_Id_Siswa = "SELECT Id FROM Siswa WHERE Id_User=@IdUser";
            int? Id_Siswa = await conn.QueryFirstAsync<int?>(Sql_Id_Siswa, new { IdUser = Id_User });
            if (!Id_Siswa.HasValue)
            {
                return (false, null, null);
            }
            string Sql_Id_Kelas = "SELECT Id_Kelas FROM Siswa WHERE Id=@IdSiswa";
            int? Id_Kelas = await conn.QueryFirstAsync<int?>(Sql_Id_Kelas, new { IdSiswa = Id_Siswa });
            if (!Id_Kelas.HasValue)
            {
                return (false, null, null);
            }
            string Sql_Id_BK = "SELECT Id_User_BK FROM Tugas_BK WHERE Id_Kelas=@IdKelas";
            int? Id_BK = await conn.QueryFirstAsync<int?>(Sql_Id_BK, new { IdKelas = Id_Kelas });
            if (!Id_BK.HasValue)
            {
                return (false, null, null);
            }
            return (true, Id_BK, Id_Siswa);
        }

        public async Task<bool> RequestTiket(int Id_User, Tiket data)
        {
            using var conn = db.connect();
            var GetIdentity = await GetIdBKAndIdSiswa(Id_User);
            string sql = @"INSERT INTO Tiket(Id_Siswa,Id_BK,Judul,Isi,Id_Status) VALUES(@Siswa,@BK,@Judul,@Isi,@Id_Status)";
            var res = await conn.ExecuteAsync(sql, new
            {
                Siswa = GetIdentity.Id_Siswa,
                BK = GetIdentity.Id_BK,
                Judul = data.Judul,
                Isi = data.Isi,
                Id_Status = 1
            });

            if (res > 0)
            {
                // get last insert id
                var newId = await conn.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");

                // try get BK's FCM token
                try
                {
                    var token = await conn.QueryFirstOrDefaultAsync<string>("SELECT FCM_Token FROM User WHERE Id = @Id", new { Id = GetIdentity.Id_BK });
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        // send notification (fire and forget)
                        _ = Task.Run(async () =>
                        {
                            await fcm.SendNotificationAsync(token, "Tiket Konseling Baru", data.Judul ?? "Anda menerima tiket baru", new Dictionary<string, string>
                            {
                                { "type", "TIKET_BARU" },
                                { "ticketId", newId.ToString() }
                            });
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending tiket notification: " + ex.Message);
                }

                return true;
            }

            return false;
        }

        public async Task<List<TiketSiswaDTO>> SiswaGet(int Id_User)
        {
            using var conn = db.connect();
            var GetIdentity = await GetIdBKAndIdSiswa(Id_User);
            string sql = @"SELECT t.Id,b.Nama AS BK,
            t.Judul,
            t.Isi,
            t.Tanggal_Pembuatan,
            t.Tanggal_Perjanjian,
            t.Tempat,
            s.Nama AS Status
            FROM Tiket t
            JOIN User b ON b.Id = t.Id_BK
            JOIN Status_Tiket s ON s.Id = t.Id_Status WHERE Id_Siswa=@Siswa";

            var res = await conn.QueryAsync<TiketSiswaDTO>(sql, new { Siswa = GetIdentity.Id_Siswa });
            return res.ToList();

        }

        public async Task<PaginatedResponse<TiketSiswaDTO>> SiswaGetPaginated(int Id_User, int page, int pageSize)
        {
            var (safePage, safePageSize) = PaginationHelper.Normalize(page, pageSize);
            int offset = (safePage - 1) * safePageSize;

            var identity = await GetIdBKAndIdSiswa(Id_User);
            if (!identity.Success || !identity.Id_Siswa.HasValue)
            {
                return new PaginatedResponse<TiketSiswaDTO>
                {
                    Page = safePage,
                    PageSize = safePageSize,
                    TotalItems = 0,
                    TotalPages = 0,
                    HasNextPage = false,
                    HasPreviousPage = false,
                    Data = new List<TiketSiswaDTO>()
                };
            }

            using var conn = db.connect();
            string countSql = @"SELECT COUNT(*) FROM Tiket WHERE Id_Siswa = @Siswa";
            string sql = @"SELECT t.Id,b.Nama AS BK,
            t.Judul,
            t.Isi,
            t.Tanggal_Pembuatan,
            t.Tanggal_Perjanjian,
            t.Tempat,
            s.Nama AS Status
            FROM Tiket t
            JOIN User b ON b.Id = t.Id_BK
            JOIN Status_Tiket s ON s.Id = t.Id_Status
            WHERE Id_Siswa = @Siswa
            ORDER BY t.Id
            LIMIT @PageSize OFFSET @Offset";

            var totalItems = await conn.ExecuteScalarAsync<int>(countSql, new { Siswa = identity.Id_Siswa });
            var rows = (await conn.QueryAsync<TiketSiswaDTO>(sql, new { Siswa = identity.Id_Siswa, PageSize = safePageSize, Offset = offset })).ToList();
            int totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / safePageSize);

            return new PaginatedResponse<TiketSiswaDTO>
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
        public async Task<bool> SiswaUpdateTiket(int Id_Tiket, Tiket data)
        {
            using var conn = db.connect();
            string sql = @"UPDATE Tiket SET Judul=@Judul,Isi=@Isi WHERE Id=@IdTiket";
            var res = await conn.ExecuteAsync(sql, new
            {
                IdTiket = Id_Tiket,
                Judul = data.Judul,
                Isi = data.Isi,
            });

            return res > 0;
        }

        public async Task<bool> SiswaDeleteTiket(int Id_Tiket)
        {

            using var conn = db.connect();
            string sql = @"DELETE FROM Tiket WHERE Id=@IdTiket";
            var res = await conn.ExecuteAsync(sql, new
            {
                IdTiket = Id_Tiket,
            });

            return res > 0;
        }



        //BK

        public async Task<List<TiketBKDTO>> BKGet(int Id_User)
        {
            using var conn = db.connect();

            string sql = @"SELECT t.Id,u.Nama AS Siswa,
            k.Tingkat,
            k.Nama AS Kelas,
            j.Kode AS Jurusan,
            t.Judul,
            t.Isi,
            t.Tanggal_Pembuatan,
            t.Tanggal_Perjanjian,
            t.Tempat,
            st.Nama AS Status
            FROM Tiket t
            JOIN Siswa si ON si.Id = t.Id_Siswa
            JOIN User u ON u.Id = si.Id_User
            JOIN Kelas k on k.Id = si.Id_Kelas
            JOIN Jurusan j ON j.Id = k.Id_Jurusan 
            JOIN Status_Tiket st ON st.Id = t.Id_Status WHERE t.Id_BK = @BK";

            var res = await conn.QueryAsync<TiketBKDTO>(sql, new { BK = Id_User });
            return res.ToList();

        }

        public async Task<PaginatedResponse<TiketBKDTO>> BKGetPaginated(int Id_User, int page, int pageSize)
        {
            var (safePage, safePageSize) = PaginationHelper.Normalize(page, pageSize);
            int offset = (safePage - 1) * safePageSize;

            using var conn = db.connect();
            string countSql = @"SELECT COUNT(*) FROM Tiket WHERE Id_BK = @BK";
            string sql = @"SELECT t.Id,u.Nama AS Siswa,
            k.Tingkat,
            k.Nama AS Kelas,
            j.Kode AS Jurusan,
            t.Judul,
            t.Isi,
            t.Tanggal_Pembuatan,
            t.Tanggal_Perjanjian,
            t.Tempat,
            st.Nama AS Status
            FROM Tiket t
            JOIN Siswa si ON si.Id = t.Id_Siswa
            JOIN User u ON u.Id = si.Id_User
            JOIN Kelas k on k.Id = si.Id_Kelas
            JOIN Jurusan j ON j.Id = k.Id_Jurusan 
            JOIN Status_Tiket st ON st.Id = t.Id_Status
            WHERE t.Id_BK = @BK
            ORDER BY t.Id
            LIMIT @PageSize OFFSET @Offset";

            var totalItems = await conn.ExecuteScalarAsync<int>(countSql, new { BK = Id_User });
            var rows = (await conn.QueryAsync<TiketBKDTO>(sql, new { BK = Id_User, PageSize = safePageSize, Offset = offset })).ToList();
            int totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / safePageSize);

            return new PaginatedResponse<TiketBKDTO>
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
        public async Task<bool> BKSetujui(int Id_Tiket, TiketUpdate data)
        {
            using var conn = db.connect();
            string sql = "UPDATE Tiket SET Id_Status=2, Tempat=@Tempat, Tanggal_Perjanjian=@Tanggal WHERE Id=@Id";
            var res = await conn.ExecuteAsync(sql, new { Id = Id_Tiket, data.Tempat, Tanggal = data.TanggalPerjanjian });

            if (res > 0)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var conn2 = db.connect();
                        var token = await conn2.QueryFirstOrDefaultAsync<string>("SELECT u.FCM_Token FROM Tiket t JOIN Siswa s ON s.Id = t.Id_Siswa JOIN User u ON u.Id = s.Id_User WHERE t.Id = @Id", new { Id = Id_Tiket });
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            await fcm.SendNotificationAsync(token, "Tiket Disetujui", "Guru BK telah menyetujui tiket Anda", new Dictionary<string, string>
                            {
                                { "type", "TIKET_STATUS" },
                                { "ticketId", Id_Tiket.ToString() },
                                { "status", "DISSETUJUI" }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending tiket status notification: " + ex.Message);
                    }
                });

                return true;
            }

            return false;
        }

        public async Task<bool> BKEditLokasi(int Id_Tiket, TiketUpdate data)
        {
            using var conn = db.connect();
            string sql = "UPDATE Tiket SET Tempat=@Tempat WHERE Id=@Id";
            var res = await conn.ExecuteAsync(sql, new { Id = Id_Tiket, data.Tempat });
            return res > 0;
        }

        public async Task<bool> BKTunda(int Id_Tiket, TiketUpdate data)
        {
            using var conn = db.connect();
            string sql = "UPDATE Tiket SET Id_Status=3, Tempat=@Tempat, Tanggal_Perjanjian=@Tanggal WHERE Id=@Id";
            var res = await conn.ExecuteAsync(sql, new { Id = Id_Tiket, data.Tempat, Tanggal = data.TanggalPerjanjian });

            if (res > 0)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var conn2 = db.connect();
                        var token = await conn2.QueryFirstOrDefaultAsync<string>("SELECT u.FCM_Token FROM Tiket t JOIN Siswa s ON s.Id = t.Id_Siswa JOIN User u ON u.Id = s.Id_User WHERE t.Id = @Id", new { Id = Id_Tiket });
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            await fcm.SendNotificationAsync(token, "Tiket Ditunda", "Guru BK menunda tiket Anda", new Dictionary<string, string>
                            {
                                { "type", "TIKET_STATUS" },
                                { "ticketId", Id_Tiket.ToString() },
                                { "status", "DITUNDA" }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending tiket status notification: " + ex.Message);
                    }
                });

                return true;
            }

            return false;
        }

        public async Task<bool> BKBatalkan(int Id_Tiket)
        {
            using var conn = db.connect();
            string sql = "UPDATE Tiket SET Id_Status=4 WHERE Id=@Id";
            var res = await conn.ExecuteAsync(sql, new { Id = Id_Tiket });

            if (res > 0)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var conn2 = db.connect();
                        var token = await conn2.QueryFirstOrDefaultAsync<string>("SELECT u.FCM_Token FROM Tiket t JOIN Siswa s ON s.Id = t.Id_Siswa JOIN User u ON u.Id = s.Id_User WHERE t.Id = @Id", new { Id = Id_Tiket });
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            await fcm.SendNotificationAsync(token, "Tiket Dibatalkan", "Guru BK membatalkan tiket Anda", new Dictionary<string, string>
                            {
                                { "type", "TIKET_STATUS" },
                                { "ticketId", Id_Tiket.ToString() },
                                { "status", "DIBATALKAN" }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending tiket status notification: " + ex.Message);
                    }
                });

                return true;
            }

            return false;
        }

        public async Task<bool> BKSelesai(int Id_Tiket)
        {
            using var conn = db.connect();
            string sql = "UPDATE Tiket SET Id_Status=5 WHERE Id=@Id";
            var res = await conn.ExecuteAsync(sql, new { Id = Id_Tiket });

            if (res > 0)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var conn2 = db.connect();
                        var token = await conn2.QueryFirstOrDefaultAsync<string>("SELECT u.FCM_Token FROM Tiket t JOIN Siswa s ON s.Id = t.Id_Siswa JOIN User u ON u.Id = s.Id_User WHERE t.Id = @Id", new { Id = Id_Tiket });
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            await fcm.SendNotificationAsync(token, "Tiket Selesai", "Tiket konseling Anda telah selesai", new Dictionary<string, string>
                            {
                                { "type", "TIKET_STATUS" },
                                { "ticketId", Id_Tiket.ToString() },
                                { "status", "SELESAI" }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending tiket status notification: " + ex.Message);
                    }
                });

                return true;
            }

            return false;
        }
    }//Class
}
