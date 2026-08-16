using BKNova.Services;
using BKNova.Models;
using System.Security.Claims;

namespace BKNova.Controllers
{
    public static class BKController
    {
        public static void MapBK(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/bk");

            g.MapPost("/", async (BKServices services, UserCreate data, IPasswordService pService) =>
            {

                try
                {
                    data.Password = pService.HashPassword(data.Password);
                    var ok = await services.Create(data);

                    if (!ok) return Results.BadRequest(new { message = "Akun Gagal Dibuat" });
                    return Results.Ok(new { message = "Akun berhasil dibuat" });
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
                }
            }).RequireAuthorization(Policies.Admin);


            g.MapGet("/", async (BKServices services) =>
            {

                try
                {

                    var ok = await services.GetAll();

                    return Results.Ok(ok);

                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
                }
            }).RequireAuthorization(Policies.Admin);
            g.MapPost("/tugas", async (BKServices services, TugasBK data) =>
           {
               try
               {
                   var ok = await services.AssignTugas(data);
                   if (!ok) return Results.BadRequest();
                   return Results.Created();
               }
               catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
           }).RequireAuthorization(Policies.Admin);

            g.MapGet("/tugas", async (BKServices services) =>
            {
                try { return Results.Ok(await services.GetAllTugas()); }
                catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
            }).RequireAuthorization(Policies.Admin);

            g.MapGet("/tugas/me", async (BKServices services, ClaimsPrincipal user) =>
            {
                try
                {
                    var idUser = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                    return Results.Ok(await services.GetTugasByBK(idUser));
                }
                catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
            }).RequireAuthorization(Policies.BK);

            g.MapPatch("/tugas/{id:int}", async (int id, UpdateTugasBK data, BKServices services) =>
            {
                try
                {
                    var ok = await services.UpdateTugas(id, data);
                    if (!ok) return Results.NotFound(new { message = $"Tugas_BK with ID {id} not found or not updated." });
                    return Results.Ok(new { message = "Tugas BK updated successfully" });
                }
                catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
            }).RequireAuthorization(Policies.Admin);

            g.MapDelete("/tugas/{id:int}", async (BKServices services, int id) =>
            {
                try
                {
                    var ok = await services.DeleteTugas(id);
                    if (!ok) return Results.BadRequest();
                    return Results.Ok();
                }
                catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
            }).RequireAuthorization(Policies.Admin);
        }
    }
}
