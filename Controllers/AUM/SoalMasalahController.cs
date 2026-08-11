using BKNova.Models;
using BKNova.Services;


namespace BKNova.Controllers
{
    public static class SoalMasalahController
    {
        public static void MapSoalMasalah(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/soal-masalah");

            g.MapGet("/", async (SoalMasalahServices services) =>
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

            g.MapGet("/{id}", async (SoalMasalahServices services,int id) =>
            {

                try
                {
                    var res = await services.GetById(id);
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
