using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BKNova.Models;
namespace BKNova.Services
{
    public interface IJWTService
    {
        string GenerateToken(User user);
    }

    public class JWTService : IJWTService
    {
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
        // Maps to User.Id
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        
        // Maps to User.Nama_Lengkap
        new Claim(ClaimTypes.Name, user.Nama),
        
        // Maps to User.Id_Role
        new Claim(ClaimTypes.Role, user.IdRole.ToString())
    };



        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.Value["JWT:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Env.Value["JWT:Issuer"],
            audience: Env.Value["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
}
