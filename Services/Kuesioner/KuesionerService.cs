using BKNova.Models;
using Dapper;

namespace BKNova.Services
{
    public class KuesionerServices
    {
        private readonly Database db;
        public KuesionerServices(Database _db) => db = _db;

        // BK - Buat Kuesioner + Soal + Opsi
        public async Task<bool> BuatKuesioner(int Id_User_BK, Kuesioner data)
        {
            using var conn = db.connect();
            await conn.OpenAsync();
            using var tx = await conn.BeginTransactionAsync();
            try
            {
                string sqlKuesioner = @"INSERT INTO Kuesioner(Id_User_BK, Id_Kelas, Id_Tahun_Ajaran, Judul, Deskripsi) 
                                VALUES(@BK, @Kelas, @TahunAjaran, @Judul, @Deskripsi);
                                SELECT LAST_INSERT_ID();";
                int Id_Kuesioner = await conn.QueryFirstAsync<int>(sqlKuesioner, new
                {
                    BK = Id_User_BK,
                    Kelas = data.Id_Kelas,
                    TahunAjaran = data.Id_Tahun_Ajaran,
                    data.Judul,
                    data.Deskripsi
                }, tx);

                foreach (var soal in data.Soal)
                {
                    string sqlSoal = @"INSERT INTO Soal_Kuesioner(Id_Kuesioner, Pertanyaan, Tipe, Urutan)
                               VALUES(@Kuesioner, @Pertanyaan, @Tipe, @Urutan);
                               SELECT LAST_INSERT_ID();";
                    int Id_Soal = await conn.QueryFirstAsync<int>(sqlSoal, new
                    {
                        Kuesioner = Id_Kuesioner,
                        soal.Pertanyaan,
                        soal.Tipe,
                        soal.Urutan
                    }, tx);

                    if (soal.Tipe == "Pilihan Ganda" && soal.Opsi.Count > 0)
                    {
                        foreach (var opsi in soal.Opsi)
                        {
                            string sqlOpsi = @"INSERT INTO Opsi_Jawaban(Id_Soal, Teks, Urutan)
                                       VALUES(@Soal, @Teks, @Urutan)";
                            await conn.ExecuteAsync(sqlOpsi, new
                            {
                                Soal = Id_Soal,
                                opsi.Teks,
                                opsi.Urutan
                            }, tx);
                        }
                    }
                }

                await tx.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                Console.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }

        // BK - Lihat daftar Kuesioner miliknya
        public async Task<List<KuesionerDTO>> BKGetList(int Id_User_BK)
        {
            using var conn = db.connect();
            string sql = @"SELECT k.Id, k.Judul, k.Deskripsi,
                            CONCAT(kl.Tingkat,' ',kl.Nama) AS Kelas,
                            ta.Nama AS Tahun_Ajaran,
                            k.Created_At
                            FROM Kuesioner k
                            JOIN Kelas kl ON kl.Id = k.Id_Kelas
                            JOIN Tahun_Ajaran ta ON ta.Id = k.Id_Tahun_Ajaran
                            WHERE k.Id_User_BK = @BK";
            var res = await conn.QueryAsync<KuesionerDTO>(sql, new { BK = Id_User_BK });
            return res.ToList();
        }

        // BK - Lihat detail Kuesioner + Soal + Opsi
        public async Task<KuesionerDetailDTO?> BKGetDetail(int Id_Kuesioner)
        {
            using var conn = db.connect();
            string sqlKuesioner = @"SELECT Id, Judul, Deskripsi FROM Kuesioner WHERE Id=@Id";
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

        // BK - Lihat jawaban per siswa
        public async Task<List<JawabanSiswaDTO>> BKGetJawaban(int Id_Kuesioner, int Id_Siswa)
        {
            using var conn = db.connect();
            string sql = @"SELECT sq.Pertanyaan, sq.Tipe,
                            oj.Teks AS Jawaban_PG,
                            jk.Teks_Jawaban AS Jawaban_Esai
                            FROM Jawaban_Kuesioner jk
                            JOIN Soal_Kuesioner sq ON sq.Id = jk.Id_Soal
                            LEFT JOIN Opsi_Jawaban oj ON oj.Id = jk.Id_Opsi
                            WHERE sq.Id_Kuesioner=@Kuesioner AND jk.Id_Siswa=@Siswa
                            ORDER BY sq.Urutan";
            var res = await conn.QueryAsync<JawabanSiswaDTO>(sql, new { Kuesioner = Id_Kuesioner, Siswa = Id_Siswa });
            return res.ToList();
        }
    }
}
