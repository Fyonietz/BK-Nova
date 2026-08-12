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
        public string Refresh_Token { get; set; } = string.Empty;
    }
    public class RefreshRequest
    {
        public string Refresh_Token { get; set; } = string.Empty;
    }
    public class Registered
    {
        public int Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Refresh_Token { get; set; } = string.Empty;
        public DateTime? Refresh_Token_Expired { get; set; }
        public string Role { get; set; }
    }

    public class MeResponse
    {
        public int Id { get; set; }
        public string Nama { get; set; } = "";
        public bool Is_Active { get; set; }
        public string Role { get; set; } = "";

        public object? Profile { get; set; }
    }

    public class SiswaProfile
    {
        public int Id { get; set; }
        public int? Id_User { get; set; }
        public string? NISN { get; set; }
        public string? NIS { get; set; }
        public string Jenis_Kelamin { get; set; } = "";
        public string Tempat_Tanggal_Lahir { get; set; } = "";
        public string Kelas {get;set;} = string.Empty;
        public string Tingkat {get;set;} = string.Empty;
        public string Jurusan {get;set;} = string.Empty;
    }
    public class WaliKelasProfile
    {
        public int Id { get; set; }
        public int? Id_User { get; set; }
        public int? Id_Kelas { get; set; }
        public int? Id_Tahun_Ajaran { get; set; }
    }
    public static class Policies
    {
        public const string Admin = "Admin";

        public static void Register(AuthorizationOptions options)
        {
            options.AddPolicy(Admin, p => p.RequireRole("Admin"));

        }
    }
}
