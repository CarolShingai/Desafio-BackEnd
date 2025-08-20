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
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Rental API - Sistema de Locação de Motos",
        Version = "v1",
        Description = @"API para gerenciamento de locação de motos.

**Funcionalidades:**
- 🏍️ Cadastro e gerenciamento de motos
- 👨‍💼 Gestão de entregadores
- 📅 Sistema de locação com planos flexíveis
- 💰 Cálculo automático de valores e multas
- 📊 Simulação de custos de devolução

**Tecnologias:**
- .NET 9
- Entity Framework Core
- PostgreSQL
- RabbitMQ
- Docker",
        Contact = new OpenApiContact
        {
            Name = "Sistema de Locação",
            Email = "contato@locacao.com"
        }
    });
    
    c.ExampleFilters();
    
    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
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
