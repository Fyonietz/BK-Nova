using BKNova.Services;
using BKNova.Models;
namespace BKNova.Controllers
{
    public static class JurusanController
    {
        public static void MapJurusan(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/jurusan");

            g.MapPost("/", async (JurusanServices services, Jurusan data) =>
            {
                try
                {
                    if (!await services.Create(data))
                    {
                        return Results.BadRequest("Failed To Create Tahun Ajaran");
                    }
                    
                    return Results.Created();
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            }).RequireAuthorization(Policies.Admin);
            g.MapGet("/", async (JurusanServices services) =>
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

            g.MapGet("/{id}", async (JurusanServices services, int Id) =>
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
            g.MapPatch("/{id}", async (JurusanServices services, int Id, Jurusan data) =>
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
            g.MapDelete("/{id}",async(JurusanServices services,int Id)=>{
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
