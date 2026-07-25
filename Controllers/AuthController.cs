using BKNova.Models;
using BKNova.Services;

namespace BKNova.Controllers{
  public static class AuthController{
    public static void MapAuth(this WebApplication app){
      var g = app.MapGroup("/api/v1/auth");

      g.MapPost("/register-admin",async (AuthServices services,Register data,IPasswordService pServices)=>{
        try{
          var Is_Registered = await services.AdminIsRegistered();
          if(Is_Registered == true){
            return Results.BadRequest("API Have Been Disabled");
          }
          data.Password = pServices.HashPassword(data.Password);
          var res = await services.RegisterAdmin(data);
          if(res == true){
            return Results.Ok();
          }else{
            return Results.BadRequest();
          }
        }catch(Exception e){
          return Results.InternalServerError(e.Message);
        }
      });
    }
  }
}
