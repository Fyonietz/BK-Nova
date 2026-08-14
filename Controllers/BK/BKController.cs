using BKNova.Services;
using BKNova.Models;

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

        }
    }
}
