using BKNova.Services;
using BKNova.Models;
namespace BKNova.Controllers
{
    public static class TahunAjaranController{
      public static void MapTahunAjaran(this WebApplication app){
        var g = app.MapGroup("/api/v1/tahun-ajaran");

        g.MapPost("/",async(TahunAjaranServices services,Tahun_Ajaran data)=>{
          try{
             if(!await services.Create(data)){
                return Results.BadRequest("Failed To Create Tahun Ajaran");
              };
              return Results.Ok("Success");
          }catch(Exception e){
            return Results.InternalServerError(e.Message);
          }
        }).RequireAuthorization(Policies.Admin);
        g.MapGet("/",async(TahunAjaranServices services)=>{
          try{
            var res = await services.GetAll();
            return Results.Ok(res);
          }catch(Exception e){
            return Results.InternalServerError(e.Message);
          }

        });

        g.MapGet("/{id}",async(TahunAjaranServices services,int Id)=>{
          try{
            var res = await services.Get(Id);
            return Results.Ok(res);
          }catch(Exception e){
            return Results.InternalServerError(e.Message);
          }

        });
      }
    }
}
