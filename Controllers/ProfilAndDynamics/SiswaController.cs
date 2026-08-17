using BKNova.Services;
using BKNova.Models;

namespace BKNova.Controllers
{
    public static class SiswaController
    {
        public static void MapSiswa(this WebApplication app)
        {
            var g = app.MapGroup("/api/v1/siswa");

            g.MapPost("/", async (SiswaServices services, RegisterSiswa data, IPasswordService pServices) =>
            {
                try
                {
                    data.user.Password = pServices.HashPassword(data.user.Password);
                    var services_data = await services.Register(data);
                    if (!services_data)
                    {
                        return Results.BadRequest();
                    }
                    return Results.Created();
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
                }
            }).RequireAuthorization(Policies.Admin);
            g.MapGet("/", async (SiswaServices services) =>
            {

                try
                {
                    var services_data = await services.GetAll();
                    return Results.Ok(services_data);
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
                }

            }).RequireAuthorization(Policies.Admin);

            g.MapGet("/kelas/{id}", async (SiswaServices services,int id) =>
            {

                try
                {
                    var services_data = await services.GetByKelas(id);
                    return Results.Ok(services_data);
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
                }

            }).RequireAuthorization(Policies.BK);
            g.MapGet("/{id}", async (SiswaServices services, int id) =>
            {

                try
                {
                    var services_data = await services.GetById(id);
                    return Results.Ok(services_data);
                }
                catch (Exception e)
                {
                    return Results.Problem(title: "Internal Server Error", statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
                }

            }).RequireAuthorization(Policies.Admin);

            g.MapPatch("/{id:int}", async (int id, UpdateSiswa data, SiswaServices services, IPasswordService pServices) =>
            {
                try
                {
                    // 1. Hash password ONLY if the user provided one
                    if (!string.IsNullOrWhiteSpace(data.Password))
                    {
                        data.Password = pServices.HashPassword(data.Password);
                    }

                    // 2. Perform the update
                    var updated = await services.Update(id, data);

                    if (!updated)
                    {
                        // Return a clear message or 404 if the record wasn't found
                        return Results.NotFound(new { message = $"Siswa or User with ID {id} was not found or not updated." });
                    }

                    // 3. Return 200 OK for successful update
                    return Results.Ok(new { message = "Siswa updated successfully" });
                }
                catch (ArgumentException ex)
                {
                    // Catch invalid enum values (e.g., Kelamin) and return 400 with the exact error message
                    return Results.BadRequest(new { error = ex.Message });
                }
                catch (Exception e)
                {
                    return Results.Problem(
                        title: "Internal Server Error",
                        statusCode: StatusCodes.Status500InternalServerError,
                        detail: e.Message
                    );
                }
            }).RequireAuthorization(Policies.Admin);

            g.MapDelete("/{id}", async (SiswaServices services, int id) =>
            {
                try
                {
                    var rest = await services.Delete(id);
                    if (!rest)
                    {
                        return Results.BadRequest();
                    }
                    return Results.Ok();
                }
                catch (Exception e)
                {

                    return Results.Problem(
                        title: "Internal Server Error",
                        statusCode: StatusCodes.Status500InternalServerError,
                        detail: e.Message
                    );

                }
            }).RequireAuthorization(Policies.Admin);
            g.MapPost("/import", async (SiswaServices services, ImportSiswaRequest data, IPasswordService pServices) =>
           {
               try
               {
                   var result = await services.ImportCsv(data, pServices);
                   return Results.Ok(result);
               }
               catch (Exception e)
               {
                   return Results.Problem(title: "Internal Server Error", statusCode: 500, detail: e.Message);
               }
           }).RequireAuthorization(Policies.Admin);
        }
    }
}
