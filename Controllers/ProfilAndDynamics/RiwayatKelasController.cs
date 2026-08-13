 using BKNova.Services;
using BKNova.Models;

namespace BKNova.Controllers
{
    public static class RiwayatKelasSiswaController
    {
        public static void MapRiwayatKelasSiswa(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/riwayat-kelas-siswa");

            g.MapPost("/", async (RiwayatKelasSiswaServices services, RiwayatKelasSiswa data) =>
            {
                try
                {
                    var ok = await services.Register(data);
                    if (!ok) return Results.BadRequest(new { message = "Siswa tidak ditemukan" });
                    return Results.Created();
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            g.MapGet("/", async (RiwayatKelasSiswaServices services) =>
            {
                try { return Results.Ok(await services.GetAll()); }
                catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
            });

            g.MapGet("/{id:int}", async (RiwayatKelasSiswaServices services, int id) =>
            {
                try { return Results.Ok(await services.GetById(id)); }
                catch (Exception e) { return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message); }
            });

            g.MapPatch("/{id:int}", async (int id, UpdateRiwayatKelasSiswa data, RiwayatKelasSiswaServices services) =>
            {
                try
                {
                    var updated = await services.Update(id, data);
                    if (!updated) return Results.NotFound(new { message = $"Riwayat_Kelas_Siswa with ID {id} not found or not updated." });
                    return Results.Ok(new { message = "Riwayat_Kelas_Siswa updated successfully" });
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            g.MapDelete("/{id:int}", async (RiwayatKelasSiswaServices services, int id) =>
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

            g.MapPost("/promote-kelas", async (RiwayatKelasSiswaServices services, PromoteKelas data) =>
            {
                try
                {
                    var count = await services.PromoteKelas(data);
                    return count switch
                    {
                        -1 => Results.NotFound(new { message = "Kelas lama tidak ditemukan" }),
                        -2 => Results.BadRequest(new { message = "Kelas ini XII, gunakan endpoint kelulusan" }),
                        -3 => Results.BadRequest(new { message = "Kelas tujuan (tingkat berikutnya) belum ada di database" }),
                        0 => Results.BadRequest(new { message = "Tidak ada siswa di kelas ini" }),
                        _ => Results.Ok(new { message = $"{count} siswa berhasil dipromosikan" })
                    };
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
                }
            }).RequireAuthorization(Policies.Admin);
        }
    }
}
