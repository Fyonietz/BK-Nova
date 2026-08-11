using BKNova.Models;
using BKNova.Services;

namespace BKNova.Controllers
{
    public static class BidangMasalahController
    {
        public static void MapBidangMasalah(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/bidang-masalah");

            g.MapGet("/", async (BidangMasalahServices services) =>
            {
                try
                {
                  var res = await services.GetAll();
                  return Results.Ok(res);
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
                }

            });
        }
    }
}
