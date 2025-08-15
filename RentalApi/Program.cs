using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using RentalApi.Application.Services;
using RentalApi.Domain.Interfaces;
using RentalApi.Domain.Entities;
using RentalApi.Infrastructure.Repositories;
using RentalApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Swagger Services
builder.Services.AddEndpointsApiExplorer(); // entrada
builder.Services.AddSwaggerGen();

// PostgreSQL
builder.Services.AddDbContext<RentalDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=rental_db;Username=postgres;Password=senha123"));

// Dependencies
builder.Services.AddScoped<IMotoRepository, MotoRepositoryEF>();
builder.Services.AddScoped<MotoService>();

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

app.MapGet("/motos", async (MotoService motoService) => {
    var motos = await motoService.GetAllMoto();
    return Results.Ok(motos);
});

app.MapGet("/motos/{id}", async (int id, MotoService motoService) => {
    var moto = await motoService.GetMotoByIdAsync(id);
    return moto is not null ? Results.Ok(moto) : Results.NotFound();
});

// Post
app.MapPost("/motos", async (MotoService motoService, Moto newMoto) =>
{
    try
    {
        var createdMoto = await motoService.RegisterNewMotoAsync(newMoto);
        return Results.Created($"/motos/{createdMoto.Id}", createdMoto);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

app.MapPut("/motos", async (int id, string license, MotoService motoService) =>
{
    try
    {
        var success = await motoService.ChangeMotoLicenseAsync(id, license);
        if (success)
        {
            return Results.Ok(new { message = "Placa atualizada com sucesso!" });
        }
        return Results.BadRequest(new { message = "Erro ao atualizar a placa" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

app.MapDelete("/motos/{id}", async (int id, MotoService motoService) =>
{
    try
    {
        await motoService.DeleteRegisteredMotoAsync(id);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

app.Run();
