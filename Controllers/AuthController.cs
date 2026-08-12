using BKNova.Models;
using BKNova.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace BKNova.Controllers
{
    public static class AuthController
    {
        public static void MapAuth(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/auth");

            g.MapPost("/register-admin", async (AuthServices services, AdminRegister data, IPasswordService pServices) =>
            {
                try
                {
                    var Is_Registered = await services.AdminIsRegistered();
                    if (Is_Registered == true)
                    {
                        return Results.BadRequest("API Have Been Disabled");
                    }
                    data.Password = pServices.HashPassword(data.Password);
                    var res = await services.RegisterAdmin(data);
                    if (res == true)
                    {
                        return Results.Ok();
                    }
                    else
                    {
                        return Results.BadRequest();
                    }
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            });
            g.MapPost("/login", async (AuthServices services, IPasswordService pServices, IJWTService jwtServices, Login login) =>
            {
                try
                {
                    var user = await services.Login(login);
                    if (user is null || !user.Is_Active)
                    {
                        return Results.Unauthorized();
                    }
                    if (!pServices.VerifyPassword(login.Password, user.Password))
                    {
                        return Results.Unauthorized();
                    }
                    var token = jwtServices.GenerateToken(user);
                    var refreshToken = jwtServices.GenerateRefreshToken();
                    await services.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(20), user.Id);
                    return Results.Ok(new LoginResponse
                    {
                        Token = token,
                        Refresh_Token = refreshToken,
                        Nama = user.Nama,
                        Role = user.Role
                    });
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            });
            g.MapPost("/refresh", async (AuthServices services, RefreshRequest req, IJWTService jwtService) =>
            {
                try
                {
                    var user = await services.RefreshTokenService(req);
                    if (user is null || user.RefreshTokenExpired < DateTime.UtcNow)
                    {
                        return Results.Unauthorized();
                    }
                    var newToken = jwtService.GenerateToken(user);
                    var newRefreshToken = jwtService.GenerateRefreshToken();

                    await services.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(20), user.Id);
                    return Results.Ok(new LoginResponse
                    {
                        Token = newToken,
                        Refresh_Token = newRefreshToken,
                        Nama = user.Nama,
                        Role = user.Role
                    });
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            });


            g.MapGet("/me", async (
                AuthServices services,
                HttpContext httpContext) =>
            {
                try
                {
                    var userIdClaim =
                        httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (!int.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var user = await services.GetMe(userId);

                    if (user is null)
                    {
                        return Results.NotFound(new
                        {
                            message = "User not found"
                        });
                    }

                    object? profile = null;

                    switch (user.Role)
                    {
                        case "Wali Kelas": // Wali Kelas
                            profile = await services.GetWaliKelasProfile(user.Id);
                            break;

                        case "Siswa": // Siswa
                            profile = await services.GetSiswaProfile(user.Id);
                            break;
                    }

                    return Results.Ok(new
                    {
                        Id = user.Id,
                        Nama = user.Nama,
                        Is_Active = user.Is_Active,
                        Role = user.Role,
                        Profile = profile
                    });
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            });

        }
    }
}
