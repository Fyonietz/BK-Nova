using BKNova.Services;
using BKNova.Models;

namespace BKNova.Controllers{
  public static class StatusTiketController{
    public static void MapStatusTiket(this WebApplication app){
      
      var g = app.MapGroup("api/v1/status-tiket");

      g.MapGet("/",async (StatusTiketServices services)=>{
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
    }
  }
}
