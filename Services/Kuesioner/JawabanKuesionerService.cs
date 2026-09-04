 using BKNova.Models;
using Dapper;

namespace BKNova.Services
{
    public class JawabanKuesionerServices
    {
        private readonly Database db;
        public JawabanKuesionerServices(Database _db) => db = _db;

        // Siswa - Lihat daftar Kuesioner yang tersedia
        public async Task<List<KuesionerDTO>> SiswaGetList(int Id_User)
        {
            using var conn = db.connect();
            string sqlSiswa = "SELECT Id, Id_Kelas FROM Siswa WHERE Id_User=@IdUser";
            var siswa = await conn.QueryFirstOrDefaultAsync<(int Id, int Id_Kelas)>(sqlSiswa, new { IdUser = Id_User });
            if (siswa == default) return new();

            string sql = @"SELECT k.Id, k.Judul, k.Deskripsi,
                            CONCAT(kl.Tingkat,' ',kl.Nama) AS Kelas,
                            ta.Nama AS Tahun_Ajaran,
                            k.Created_At,
                            CASE WHEN sk.Id IS NOT NULL THEN 1 ELSE 0 END AS Sudah_Submit
                            FROM Kuesioner k
                            JOIN Kelas kl ON kl.Id = k.Id_Kelas
                            JOIN Tahun_Ajaran ta ON ta.Id = k.Id_Tahun_Ajaran
                            LEFT JOIN Status_Submit_Kuesioner sk 
                                ON sk.Id_Kuesioner = k.Id AND sk.Id_Siswa = @Siswa
                            WHERE k.Id_Kelas = @Kelas";
            var res = await conn.QueryAsync<KuesionerDTO>(sql, new { Siswa = siswa.Id, Kelas = siswa.Id_Kelas });
            return res.ToList();
        }

        // Siswa - Lihat detail Kuesioner (untuk diisi)
        public async Task<KuesionerDetailDTO?> SiswaGetDetail(int Id_Kuesioner)
        {
            using var conn = db.connect();
            string sqlKuesioner = "SELECT Id, Judul, Deskripsi FROM Kuesioner WHERE Id=@Id";
            var kuesioner = await conn.QueryFirstOrDefaultAsync<KuesionerDetailDTO>(sqlKuesioner, new { Id = Id_Kuesioner });
            if (kuesioner == null) return null;

            string sqlSoal = @"SELECT Id, Pertanyaan, Tipe, Urutan
                                FROM Soal_Kuesioner WHERE Id_Kuesioner=@Kuesioner
                                ORDER BY Urutan";
            var soalList = (await conn.QueryAsync<SoalDetailDTO>(sqlSoal, new { Kuesioner = Id_Kuesioner })).ToList();

            foreach (var soal in soalList)
            {
if (soal.Tipe == "Pilihan Ganda")
                {
                    string sqlOpsi = @"SELECT Id, Teks, Urutan FROM Opsi_Jawaban
                                       WHERE Id_Soal=@Soal ORDER BY Urutan";
                    soal.Opsi = (await conn.QueryAsync<OpsiDetailDTO>(sqlOpsi, new { Soal = soal.Id })).ToList();
                }
            }

            kuesioner.Soal = soalList;
            return kuesioner;
        }

        // Siswa - Submit jawaban
        public async Task<bool> SiswaSubmit(int Id_User, int Id_Kuesioner, List<JawabanKuesioner> data)
        {
            using var conn = db.connect();
            string sqlSiswa = "SELECT Id FROM Siswa WHERE Id_User=@IdUser";
            int? Id_Siswa = await conn.QueryFirstOrDefaultAsync<int?>(sqlSiswa, new { IdUser = Id_User });
            if (!Id_Siswa.HasValue) return false;

            // Cek sudah submit belum
            string sqlCek = "SELECT Id FROM Status_Submit_Kuesioner WHERE Id_Siswa=@Siswa AND Id_Kuesioner=@Kuesioner";
            var sudahSubmit = await conn.QueryFirstOrDefaultAsync<int?>(sqlCek, new { Siswa = Id_Siswa, Kuesioner = Id_Kuesioner });
            if (sudahSubmit.HasValue) return false;

            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                foreach (var jawaban in data)
                {
                    string sqlJawaban = @"INSERT INTO Jawaban_Kuesioner(Id_Siswa, Id_Soal, Id_Opsi, Teks_Jawaban)
                                          VALUES(@Siswa, @Soal, @Opsi, @Teks)";
                    await conn.ExecuteAsync(sqlJawaban, new
                    {
                        Siswa = Id_Siswa,
                        Soal = jawaban.Id_Soal,
                        Opsi = jawaban.Id_Opsi,
                        Teks = jawaban.Teks_Jawaban
                    }, tx);
                }

                string sqlStatus = @"INSERT INTO Status_Submit_Kuesioner(Id_Siswa, Id_Kuesioner)
                                     VALUES(@Siswa, @Kuesioner)";
                await conn.ExecuteAsync(sqlStatus, new { Siswa = Id_Siswa, Kuesioner = Id_Kuesioner }, tx);

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                return false;
            }
        }
    }
}
