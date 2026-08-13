using BKNova.Models;
using BKNova.Services;

namespace BKNova.Controllers
{
    public static class AumController
    {
        public static void MapAum(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/aum");

            g.MapPost("/submit", async (AumServices services, SubmitAUM data) =>
           {
               try
               {
                   var ok = await services.SubmitAUM(data);
                   if (!ok) return Results.BadRequest(new { message = "Sudah submit atau siswa tidak ditemukan" });
                   return Results.Ok(new { message = "AUM berhasil disubmit" });
               }
               catch (Exception e)
               {
                   return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
               }
           }).RequireAuthorization(Policies.Siswa);
            g.MapGet("/status/{idUser:int}/{idTahunAjaran:int}", async (AumServices services, int idUser, int idTahunAjaran) =>
           {
               try { return Results.Ok(new { submitted = await services.HasSubmitted(idUser, idTahunAjaran) }); }
               catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
           });
        }
    }
}
