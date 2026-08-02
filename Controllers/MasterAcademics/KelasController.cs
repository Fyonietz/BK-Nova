
using BKNova.Services;
using BKNova.Models;
namespace BKNova.Controllers
{
    public static class KelasController
    {
        public static void MapKelas(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/kelas");

            g.MapPost("/", async (KelasServices services, Kelas data) =>
            {
                try
                {
                    if (!await services.Create(data))
                    {
                        return Results.BadRequest("Failed To Create Kelas");
                    }
                    
                    return Results.Created();
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            }).RequireAuthorization(Policies.Admin);
            g.MapGet("/", async (KelasServices services) =>
            {
                try
                {
                    var res = await services.GetAll();
                    return Results.Ok(res);
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }

            });

            g.MapGet("/{id}", async (KelasServices services, int Id) =>
            {
                try
                {
                    var res = await services.Get(Id);
                    return Results.Ok(res);
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }

            });
            g.MapPatch("/{id}", async (KelasServices services, int Id, Kelas data) =>
            {
                try
                {
                  var res = await services.Update(Id,data);
                  if(!res){
                    return Results.Problem("Failed To Update");
                  }
                  return Results.Ok();
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);

                }
            }).RequireAuthorization(Policies.Admin);
            g.MapDelete("/{id}",async(KelasServices services,int Id)=>{
                try{
                  var res = await services.Delete(Id);
                  if(!res){
                    return Results.Problem("Failed To Delete");
                  }
                  return Results.Ok();
                }catch(Exception e){
                  return Results.InternalServerError(e.Message);
                }
            }).RequireAuthorization(Policies.Admin);
        }
    }
}
