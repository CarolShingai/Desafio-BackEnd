using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using RentalApi.Application.Services;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Repositories;
using RentalApi.Infrastructure.Data;
using RentalApi.Infrastructure.Messaging;
using RentalApi.Infrastructure.Background;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Swagger Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.ExampleFilters();
});
builder.Services
    .AddSwaggerExamplesFromAssemblyOf<RentalApi.Application.DTOs.CreateRentalRequestExample>();

// PostgreSQL
builder.Services.AddDbContext<RentalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependencies
builder.Services.AddScoped<IMotoRepository, MotoRepositoryEF>();
builder.Services.AddScoped<MotoService>();
builder.Services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

// DeliveryPerson Dependencies
builder.Services.AddScoped<IDeliveryPersonRepository, DeliveryRepository>();
builder.Services.AddScoped<IDeliveryPersonService, DeliveryPersonService>();

// RentMoto Dependencies
builder.Services.AddScoped<IRentMotoRepository, RentMotoRepository>();
builder.Services.AddScoped<IRentMotoService, RentMotoService>();

// Background Services
builder.Services.AddHostedService<RabbitMqBackgroundService>();

//Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RentalDbContext>();
    context.Database.Migrate();
}

// Activate Swagger over development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

public partial class Program { }
