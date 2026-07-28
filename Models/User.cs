namespace BKNova.Models{
  public class User{
    public int Id {get;set;}
    public string Role {get;set;}
    public string Nama {get;set;} = string.Empty;
    public string Password {get;set;}= string.Empty;
    public string RefreshToken {get;set;}= string.Empty;
    public DateTime? RefreshTokenExpired {get;set;}
    public bool Is_Active {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
    public DateTime UpdatedAt {get;set;} = DateTime.UtcNow;
  }
}
