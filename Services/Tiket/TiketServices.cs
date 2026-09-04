
using BKNova.Models;
using Dapper;


namespace BKNova.Services
{
    public class TiketServices
    {

        private readonly Database db;
        public TiketServices(Database _db) => db = _db;


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

            return res > 0;
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
        public async Task<bool> BKSetujui(int Id_Tiket, TiketUpdate data)
        {
            using var conn = db.connect();
            string sql = "UPDATE Tiket SET Id_Status=2, Tempat=@Tempat, Tanggal_Perjanjian=@Tanggal WHERE Id=@Id";
            var res = await conn.ExecuteAsync(sql, new { Id = Id_Tiket, data.Tempat, Tanggal = data.TanggalPerjanjian });
            return res > 0;
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
            return res > 0;
        }

        public async Task<bool> BKBatalkan(int Id_Tiket)
        {
            using var conn = db.connect();
            string sql = "UPDATE Tiket SET Id_Status=4 WHERE Id=@Id";
            var res = await conn.ExecuteAsync(sql, new { Id = Id_Tiket });
            return res > 0;
        }

        public async Task<bool> BKSelesai(int Id_Tiket)
        {
            using var conn = db.connect();
            string sql = "UPDATE Tiket SET Id_Status=5 WHERE Id=@Id";
            var res = await conn.ExecuteAsync(sql, new { Id = Id_Tiket });
            return res > 0;
        }
    }//Class
}
