using BKNova.Services;
using BKNova.Models;
using BKNova.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Diagnostics;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
Env.Value = builder.Configuration;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAndroid", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddMemoryCache();
// Add services to the container.
builder.Services.AddSingleton<Database>();
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});
builder.Services.AddAuthorization(Policies.Register);
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJWTService, JWTService>();

//CRUD Services Registration
builder.Services.AddScoped<AuthServices>();

//Master Academics
builder.Services.AddScoped<TahunAjaranServices>();
builder.Services.AddScoped<JurusanServices>();
builder.Services.AddScoped<KelasServices>();

//Profil And Dynamics
builder.Services.AddScoped<SiswaServices>();
builder.Services.AddScoped<WaliKelasServices>();

//AUM
builder.Services.AddScoped<BidangMasalahServices>();
builder.Services.AddScoped<SoalMasalahServices>();
builder.Services.AddScoped<AumServices>();
var app = builder.Build();
app.UseCors("AllowAndroid");
//Logger
app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();

    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
    finally
    {
        sw.Stop();

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Console.WriteLine(
            $"{timestamp} INFO: {ip} - \"{context.Request.Method} {context.Request.Path} {context.Response.StatusCode}\" {sw.ElapsedMilliseconds}ms"
        );
    }
});
app.UseAuthentication();
app.UseAuthorization();
//CRUD Controller Registration
app.MapAuth();
app.MapTahunAjaran();
app.MapJurusan();
app.MapKelas();
app.MapSiswa();

//AUM
app.MapBidangMasalah();
app.MapSoalMasalah();
app.MapAum();
app.Run();
