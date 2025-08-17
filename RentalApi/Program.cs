using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using RentalApi.Application.Services;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Repositories;
using RentalApi.Infrastructure.Data;
using RentalApi.Infrastructure.Messaging;
using RentalApi.Infrastructure.Background;

var builder = WebApplication.CreateBuilder(args);

// Swagger Services
builder.Services.AddEndpointsApiExplorer(); // entrada
builder.Services.AddSwaggerGen();

// PostgreSQL
builder.Services.AddDbContext<RentalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependencies
builder.Services.AddScoped<IMotoRepository, MotoRepositoryEF>();
builder.Services.AddScoped<MotoService>();
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IMessageConsumer, RabbitMqConsumer>();

// Background Services
builder.Services.AddHostedService<RabbitMqBackgroundService>();

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


app.MapControllers();
app.Run();

public partial class Program { }
