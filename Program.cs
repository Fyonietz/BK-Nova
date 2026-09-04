using BKNova.Services;
using BKNova.Models;
using BKNova.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Diagnostics;
using System.Text.Json.Serialization;

// Namespace ini akan valid karena kita menggunakan Swashbuckle
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
Env.Value = builder.Configuration;

// ─────────────────────────────────────────────
// 1. CORS CONFIGURATION
// ─────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAndroid", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy("AllowWebFrontend", policy =>
    {
        policy.WithOrigins(
                    "http://localhost:3000",   
                    "http://localhost:5173",   
                    "http://localhost:4200",
                    "http://192.168.69.50:5173",
                    "https://yourdomain.com"   
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); 
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<Database>();

// ─────────────────────────────────────────────
// 2. AUTHENTICATION (JWT)
// ─────────────────────────────────────────────
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
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// ─────────────────────────────────────────────
// 3. SWAGGER CONFIGURATION
// ─────────────────────────────────────────────

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "BKNova API", 
        Version = "v1",
        Description = "Dokumentasi API BKNova"
    });

    // UBAH BAGIAN INI: Gunakan Type = Http
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Masukkan JWT Token saja. Swagger akan otomatis menambahkan kata 'Bearer ' di depannya.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, // <-- Diubah dari ApiKey ke Http
        Scheme = "Bearer",              // <-- Wajib lowercase atau PascalCase "Bearer"
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────
// 4. SERVICES REGISTRATION
// ─────────────────────────────────────────────
builder.Services.AddAuthorization(Policies.Register);
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJWTService, JWTService>();

builder.Services.AddScoped<AuthServices>();
builder.Services.AddScoped<TahunAjaranServices>();
builder.Services.AddScoped<JurusanServices>();
builder.Services.AddScoped<KelasServices>();
builder.Services.AddScoped<SiswaServices>();
builder.Services.AddScoped<WaliKelasServices>();
builder.Services.AddScoped<RiwayatKelasSiswaServices>();
builder.Services.AddScoped<BidangMasalahServices>();
builder.Services.AddScoped<SoalMasalahServices>();
builder.Services.AddScoped<AumServices>();
builder.Services.AddScoped<BKServices>();
//Tiket
builder.Services.AddScoped<StatusTiketServices>();
builder.Services.AddScoped<TiketServices>();

//Kuesioner
builder.Services.AddScoped<KuesionerServices>();
builder.Services.AddScoped<JawabanKuesionerServices>();
var app = builder.Build();

// ─────────────────────────────────────────────
// 5. MIDDLEWARE PIPELINE
// ─────────────────────────────────────────────
    // Mengaktifkan UI Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BKNova API v1");
    });

app.UseCors("AllowWebFrontend");

// Logger
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
        Console.WriteLine($"{timestamp} INFO: {ip} - \"{context.Request.Method} {context.Request.Path} {context.Response.StatusCode}\" {sw.ElapsedMilliseconds}ms");
    }
});

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/",()=>{
    return Results.Ok("Server Running");
});
// ─────────────────────────────────────────────
// 6. ENDPOINT MAPPING
// ─────────────────────────────────────────────
app.MapAuth();
app.MapTahunAjaran();
app.MapJurusan();
app.MapKelas();
app.MapSiswa();
app.MapWaliKelas();
app.MapRiwayatKelasSiswa();
app.MapBidangMasalah();
app.MapSoalMasalah();
app.MapAum();
app.MapBK();
app.MapStatusTiket();
app.MapTiket();
app.MapKuesioner();
app.Run();
