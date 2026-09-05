using BKNova.Services;
using BKNova.Models;

namespace BKNova.Controllers
{
    public static class KuesionerController
    {
        public static void MapKuesioner(this WebApplication app)
        {
            var g = app.MapGroup("api/v1/kuesioner");

            // BK
            g.MapPost("/{IdUser}", async (KuesionerServices services, Kuesioner data, int IdUser) =>
            {
                try
                {
                    var res = await services.BuatKuesioner(IdUser, data);
                    if (!res) return Results.Problem();
                    return Results.Created();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"CONTROLLER ERROR: {e.Message}");
                    Console.WriteLine(e.StackTrace);
                    return Results.InternalServerError(e.Message);
                }
            });
            // .RequireAuthorization(Policies.BK);

            g.MapGet("/bk/{IdUser}", async (KuesionerServices services, int IdUser) =>
            {
                try
                {
                    var res = await services.BKGetList(IdUser);
                    return Results.Ok(res);
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.BK);

            g.MapGet("/bk/detail/{IdKuesioner}", async (KuesionerServices services, int IdKuesioner) =>
            {
                try
                {
                    var res = await services.BKGetDetail(IdKuesioner);
                    if (res == null) return Results.NotFound();
                    return Results.Ok(res);
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.BK);

            g.MapGet("/bk/jawaban/{IdKuesioner}/{IdSiswa}", async (KuesionerServices services, int IdKuesioner, int IdSiswa) =>
            {
                try
                {
                    var res = await services.BKGetJawaban(IdKuesioner, IdSiswa);
                    return Results.Ok(res);
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.BK);

            // Siswa
            g.MapGet("/siswa/{IdUser}", async (JawabanKuesionerServices services, int IdUser) =>
            {
                try
                {
                    var res = await services.SiswaGetList(IdUser);
                    return Results.Ok(res);
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.Siswa);

            g.MapGet("/siswa/detail/{IdKuesioner}", async (JawabanKuesionerServices services, int IdKuesioner) =>
            {
                try
                {
                    var res = await services.SiswaGetDetail(IdKuesioner);
                    if (res == null) return Results.NotFound();
                    return Results.Ok(res);
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.Siswa);

            g.MapPost("/siswa/submit/{IdUser}/{IdKuesioner}", async (JawabanKuesionerServices services, List<JawabanKuesioner> data, int IdUser, int IdKuesioner) =>
            {
                try
                {
                    var res = await services.SiswaSubmit(IdUser, IdKuesioner, data);
                    if (!res) return Results.Problem();
                    return Results.Created();
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.Siswa);
        }
    }
}
