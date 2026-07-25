using Dapper;
using BKNova.Models;
namespace BKNova.Services{
  public class AuthServices{
    private readonly Database db;
    public AuthServices(Database _db)=>db=_db;

    public async Task<bool> RegisterAdmin(Register data){
      using var conn = db.connect();
      string sql = @"INSERT INTO User(Nama,Id_Role,Password,Is_Active) VALUES(@Nama,@Id_Role,@Password,@Is_Active)";
      return await conn.ExecuteAsync(sql,new {
        Nama = data.Nama,
        Id_Role = 1,
        Password = data.Password,
        Is_Active = true
      }) > 0;
    }

    public async Task<bool> AdminIsRegistered(){
      using var conn = db.connect();
      var count =await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM User");
      return count > 0;
    }
  }
}
