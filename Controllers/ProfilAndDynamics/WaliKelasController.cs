using BKNova.Services;
using BKNova.Models;

namespace BKNova.Controllers
{
    public static class WaliKelasController
    {
        public static void MapWaliKelas(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/wali-kelas");

            g.MapPost("/", async (WaliKelasServices services, RegisterWaliKelas data, IPasswordService pServices) =>
            {
                try
                {
                    data.user.Password = pServices.HashPassword(data.user.Password);
                    var ok = await services.Register(data);
                    if (!ok) return Results.BadRequest();
                    return Results.Created();
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            g.MapGet("/", async (WaliKelasServices services) =>
            {
                try { return Results.Ok(await services.GetAll()); }
                catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
            });

            g.MapGet("/{id:int}", async (WaliKelasServices services, int id) =>
            {
                try { return Results.Ok(await services.GetById(id)); }
                catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
            });

            g.MapPatch("/{id:int}", async (int id, UpdateWaliKelas data, WaliKelasServices services,IPasswordService pServices) =>
            {
                try
                { 
                    data.Password = pServices.HashPassword(data.Password);
                    var updated = await services.Update(id, data);
                    if (!updated) return Results.NotFound(new { message = $"Wali_Kelas with ID {id} not found or not updated." });
                    return Results.Ok(new { message = "Wali_Kelas updated successfully" });
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            g.MapDelete("/{id:int}", async (WaliKelasServices services, int id) =>
            {
                try
                {
                    var ok = await services.Delete(id);
                    if (!ok) return Results.BadRequest();
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
                }
            }).RequireAuthorization(Policies.Admin);
        }
    }
}
