using BKNova.Models;
using Dapper;


namespace BKNova.Services{
  public class StatusTiketServices{

        private readonly Database db;
        public StatusTiketServices(Database _db) => db = _db;

        public async Task<List<StatusTiketDTO>> GetAll(){
          using var conn = db.connect();
          string sql = "SELECT Id,Nama FROM Status_Tiket";
          var res = await conn.QueryAsync<StatusTiketDTO>(sql);
          return res.ToList();
        }
  }
}
