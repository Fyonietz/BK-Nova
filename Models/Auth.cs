using Microsoft.AspNetCore.Authorization;

namespace BKNova.Models
{
    public class AdminRegister
    {
        public string Nama { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class Login
    {
        public string Nama { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
    public class Registered
    {
        public int Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpired { get; set; }
        public string Role { get; set; }
    }
   public static class Policies{
     public const string Admin = "Admin";

     public static void Register(AuthorizationOptions options){
       options.AddPolicy(Admin,p => p.RequireRole("Admin"));

     }
   }
}
