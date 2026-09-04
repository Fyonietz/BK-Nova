
using BKNova.Services;
using BKNova.Models;

namespace BKNova.Controllers
{
    public static class TiketController
    {
        public static void MapTiket(this WebApplication app)
        {

            var g = app.MapGroup("api/v1/tiket");

            //Siswa
            g.MapPost("/request/{IdSiswa}", async (TiketServices services, Tiket data, int IdSiswa) =>
            {
                try
                {
                    var res = await services.RequestTiket(IdSiswa, data);
                    if (!res)
                    {
                        return Results.Problem();
                    }
                    return Results.Created();
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            }).RequireAuthorization(Policies.Siswa);

            g.MapGet("/{IdUser}", async (TiketServices services, int IdUser) =>
            {
                try
                {
                    var res = await services.SiswaGet(IdUser);
                    return Results.Ok(res);
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            }).RequireAuthorization(Policies.Siswa);
            g.MapPatch("/{IdTiket}", async (TiketServices services, Tiket data, int IdTiket) =>
            {

                try
                {
                    var res = await services.SiswaUpdateTiket(IdTiket, data);
                    if (!res)
                    {
                        return Results.Problem();
                    }
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }

            }).RequireAuthorization(Policies.Siswa);

            g.MapDelete("/{IdTiket}", async (TiketServices services, int IdTiket) =>
            {

                try
                {
                    var res = await services.SiswaDeleteTiket(IdTiket);
                    if (!res)
                    {
                        return Results.Problem();
                    }
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }

            }).RequireAuthorization(Policies.Siswa);


            //BK

            g.MapGet("/bk/{IdUser}", async (TiketServices services, int IdUser) =>
            {
                try
                {
                    var res = await services.BKGet(IdUser);
                    return Results.Ok(res);
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            }).RequireAuthorization(Policies.BK);


            g.MapPatch("/bk/setujui/{IdTiket}", async (TiketServices services, TiketUpdate data, int IdTiket) =>
            {
                try
                {
                    var res = await services.BKSetujui(IdTiket, data);
                    if (!res) return Results.Problem();
                    return Results.Ok();
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.BK);

            g.MapPatch("/bk/lokasi/{IdTiket}", async (TiketServices services, TiketUpdate data, int IdTiket) =>
            {
                try
                {
                    var res = await services.BKEditLokasi(IdTiket, data);
                    if (!res) return Results.Problem();
                    return Results.Ok();
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.BK);

            g.MapPatch("/bk/tunda/{IdTiket}", async (TiketServices services, TiketUpdate data, int IdTiket) =>
            {
                try
                {
                    var res = await services.BKTunda(IdTiket, data);
                    if (!res) return Results.Problem();
                    return Results.Ok();
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.BK);

            g.MapPatch("/bk/batalkan/{IdTiket}", async (TiketServices services, int IdTiket) =>
            {
                try
                {
                    var res = await services.BKBatalkan(IdTiket);
                    if (!res) return Results.Problem();
                    return Results.Ok();
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.BK);

            g.MapPatch("/bk/selesai/{IdTiket}", async (TiketServices services, int IdTiket) =>
            {
                try
                {
                    var res = await services.BKSelesai(IdTiket);
                    if (!res) return Results.Problem();
                    return Results.Ok();
                }
                catch (Exception e) { return Results.InternalServerError(e.Message); }
            }).RequireAuthorization(Policies.BK);
        }
    }
}
