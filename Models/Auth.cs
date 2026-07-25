namespace BKNova.Models
{
  public class Register{
    public string Nama {get;set;} = string.Empty;
    public string Password {get;set;} = string.Empty;
  }
  public class Registered{
    public int Id {get;set;}
    public string Nama {get;set;} = string.Empty;
    public string RefreshToken {get;set;}= string.Empty;
    public DateTime? RefreshTokenExpired {get;set;}
    public string Role {get;set;}
  }
}
