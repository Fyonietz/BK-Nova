using BKNova.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;


namespace BKNova.Services
{
    public class SoalMasalahServices
    {
        private readonly IMemoryCache cache;
        private readonly Database db;
        public const string CACHE_KEY = "soal_masalah_all";
        public SoalMasalahServices(Database _db, IMemoryCache _cache)
        {
            db = _db;
            cache = _cache;
        }

        public async Task<List<SoalMasalahDTO>> GetAll()
        {
            if (cache.TryGetValue(CACHE_KEY, out List<SoalMasalahDTO>? cached))
                return cached!;

            using var conn = db.connect();
            string sql = @"SELECT s.Id,
            b.Kode AS Kode,
            b.Nama AS BidangMasalah,
            s.Pertanyaan 
            FROM Soal_Masalah s 
            JOIN Bidang_Masalah b ON s.Id_Bidang_Masalah = b.Id;";
            var result = await conn.QueryAsync<SoalMasalahDTO>(sql);
            cache.Set(CACHE_KEY, result, TimeSpan.FromHours(6));
            return result.ToList();
        }

        public async Task<List<SoalMasalahDTO>?> GetById(int Id)
        {
            using var conn = db.connect();
            string sql = @"SELECT s.Id,
            b.Kode AS Kode,
            b.Nama AS BidangMasalah,
            s.Pertanyaan 
            FROM Soal_Masalah s 
            JOIN Bidang_Masalah b ON s.Id_Bidang_Masalah = b.Id
            WHERE s.Id_Bidang_Masalah = @Id";

            var result = await conn.QueryAsync<SoalMasalahDTO>(sql,new {
                Id = Id
                });
            return result.ToList();
        }
    }
}
