using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using RentalApi.Application.Services;
using RentalApi.Domain.Interfaces;
using RentalApi.Domain.Entities;
using RentalApi.Infrastructure.Repositories;
using RentalApi.Infrastructure.Data;
using RentalApi.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Swagger Services
builder.Services.AddEndpointsApiExplorer(); // entrada
builder.Services.AddSwaggerGen();

// PostgreSQL
builder.Services.AddDbContext<RentalDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=rental_db;Username=postgres;Password=senha123"));

// Dependencies
builder.Services.AddScoped<IMotoRepository, MotoRepositoryEF>();
builder.Services.AddScoped<IMotoService, MotoService>();

//Controllers
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RentalDbContext>();
    context.Database.EnsureCreated();
}

// Activate Swagger over development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Post
// app.MapPost("/motos", async (CreateMotoRequest request, MotoService motoService) =>
// {
//     var moto = new Moto
//     {
//         Identificador = request.Identificador,
//         Ano = request.Ano,
//         Modelo = request.Modelo,
//         Placa = request.Placa
//     };

//     var createdMoto = await motoService.RegisterNewMotoAsync(moto);

//     var response = new MotoResponse
//     {
//         Identificador = request.Identificador,
//         Ano = request.Ano,
//         Modelo = request.Modelo,
//         Placa = request.Placa
//     };

//     return Results.Created($"/motos/{createdMoto.Identificador}", response);
// });

// app.MapGet("/motos", async (MotoService motoService) =>
// {
//     var motos = await motoService.GetAllMoto();

//     var response = motos.Select(m => new MotoResponse
//     {
//         Identificador = m.Identificador,
//         Ano = m.Ano,
//         Modelo = m.Modelo,
//         Placa = m.Placa
//     });

//     return Results.Ok(response);
// });

// app.MapGet("/motos/{id}", async (string id, MotoService motoService) =>
// {
//     try
//     {
//         if (string.IsNullOrWhiteSpace(id))
//             return Results.BadRequest(new { message = "Request mal Formada" });

//         var moto = await motoService.GetMotoByIdentifierAsync(id);
//         if (moto == null)
//             return Results.NotFound(new { message = "Moto não encontrada" });

//         var response = new MotoResponse
//         {
//             Identificador = moto.Identificador,
//             Ano = moto.Ano,
//             Modelo = moto.Modelo,
//             Placa = moto.Placa
//         };
//         return Results.Ok(response);
//     }
//     catch (Exception ex)
//     {
//         return Results.BadRequest(new { message = ex.Message });
//     }
// });

// app.MapPut("/motos/{id}/placa", async (string id, string license, MotoService motoService) =>
// {
//     try
//     {
//         var success = await motoService.ChangeMotoLicenseAsync(id, license);
//         if (success)
//         {
//             return Results.Ok(new { message = "Placa modificada com sucesso" });
//         }
//         return Results.BadRequest(new { message = "Erro ao atualizar a placa" });
//     }
//     catch (Exception ex)
//     {
//         return Results.BadRequest(new { message = ex.Message });
//     }
// });

// app.MapDelete("/motos/{id}", async (string id, MotoService motoService) =>
// {
//     try
//     {
//         await motoService.DeleteRegisteredMotoAsync(id);
//         return Results.NoContent();
//     }
//     catch (Exception ex)
//     {
//         return Results.BadRequest(new { message = ex.Message });
//     }
// });

app.MapControllers();
app.Run();

public partial class Program { }
