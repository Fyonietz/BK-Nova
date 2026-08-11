 using BKNova.Models;
 using Dapper;

 namespace BKNova.Services
 {
    public class BidangMasalahServices{
      private readonly Database db;
      public BidangMasalahServices(Database _db) => db = _db;

      public async Task<List<BidangMasalahDTO>> GetAll(){
        using var conn = db.connect();
        string sql = @"SELECT * FROM Bidang_Masalah";
        var result = await conn.QueryAsync<BidangMasalahDTO>(sql);
        return result.ToList();
      }
    }   
 }
