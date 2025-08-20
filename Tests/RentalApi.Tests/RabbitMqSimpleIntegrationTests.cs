using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;
using RentalApi.Infrastructure.Messaging;
using RentalApi.Infrastructure.Repositories;
using RentalApi.Infrastructure.Background;
using System.Text.Json;
using Xunit;

namespace RentalApi.Tests
{
    public class RabbitMqSimpleIntegrationTests : IAsyncLifetime
    {
        private IServiceProvider? _serviceProvider;
        private IHost? _host;

        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Port=5432;Database=test_rental;Username=postgres;Password=postgres;",
                    ["RabbitMQ:Host"] = "localhost",
                    ["RabbitMQ:Username"] = "guest",
                    ["RabbitMQ:Password"] = "guest"
                })
                .Build();

            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    
                    // Configurar Entity Framework com InMemory para testes rápidos
                    services.AddDbContext<RentalDbContext>(options =>
                        options.UseInMemoryDatabase("TestDatabase"));
                    
                    // Registrar serviços
                    services.AddScoped<IMotoNotificationRepository, MotoNotificationRepository>();
                    services.AddScoped<IMessageConsumer, RabbitMqConsumer>();
                    services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
                    
                    // Mock do IMotoRepository
                    services.AddScoped<IMotoRepository>(provider => 
                    {
                        var mock = new Moq.Mock<IMotoRepository>();
                        return mock.Object;
                    });
                    
                    // Registrar o Background Service
                    services.AddHostedService<RabbitMqBackgroundService>();
                });

            _host = hostBuilder.Build();
            _serviceProvider = _host.Services;

            // Criar o banco de dados
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RentalDbContext>();
            await context.Database.EnsureCreatedAsync();
            
            // Iniciar o host para que o background service funcione
            await _host.StartAsync();
        }

        [Fact]
        public async Task RabbitMqPublisher_ShouldPublishMessage_Successfully()
        {
            // Arrange
            using var scope = _serviceProvider!.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            
            var moto2024 = new Moto
            {
                Identifier = Guid.NewGuid().ToString(),
                Year = 2024,
                MotorcycleModel = "Honda CB 600F",
                LicensePlate = "ABC1234"
            };

            // Act & Assert - Verificar se a publicação não gera exceção
            var exception = await Record.ExceptionAsync(async () =>
            {
                await publisher.PublishAsync(moto2024, "motoQueue");
            });

            Assert.Null(exception);
        }

        [Fact]
        public async Task MotoNotificationRepository_ShouldSaveAndRetrieve_Correctly()
        {
            // Arrange
            using var scope = _serviceProvider!.CreateScope();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<IMotoNotificationRepository>();
            
            var notification = new MotoNotification
            {
                Id = Guid.NewGuid(),
                MotorcycleId = 123,
                Year = 2024,
                Model = "Test Model",
                LicensePlate = "TEST123",
                Message = "Test notification",
                NotifiedAt = DateTime.UtcNow
            };

            // Act
            var savedNotification = await notificationRepo.AddNotificationAsync(notification);
            var allNotifications = await notificationRepo.GetAllNotificationsAsync();

            // Assert
            Assert.NotNull(savedNotification);
            Assert.Equal(notification.Id, savedNotification.Id);
            Assert.Contains(allNotifications, n => n.Id == notification.Id);
        }

        [Fact]
        public async Task BackgroundService_Functionality_ShouldBeConfigured()
        {
            // Arrange & Act - Verificar se o background service está configurado
            using var scope = _serviceProvider!.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
            var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<IMotoNotificationRepository>();

            // Assert - Verificar se todos os serviços necessários estão disponíveis
            Assert.NotNull(consumer);
            Assert.NotNull(publisher);
            Assert.NotNull(notificationRepo);
        }

        [Fact]
        public async Task MotoNotification_Constructor_ShouldCreateValid_Notification()
        {
            // Arrange
            var motorcycleId = 123;
            var motorcycleIdentifier = "test-moto-123";
            var year = 2024;
            var model = "Honda CB 600F";
            var licensePlate = "ABC1234";

            // Act
            var notification = new MotoNotification(motorcycleId, motorcycleIdentifier, year, model, licensePlate);

            // Assert
            Assert.NotEqual(Guid.Empty, notification.Id);
            Assert.Equal(motorcycleId, notification.MotorcycleId);
            Assert.Equal(year, notification.Year);
            Assert.Equal(model, notification.Model);
            Assert.Equal(licensePlate, notification.LicensePlate);
            Assert.Contains($"Moto de {year} foi registrada: {model} - {licensePlate}", notification.Message);
            Assert.True(notification.NotifiedAt <= DateTime.UtcNow);
        }

        public async Task DisposeAsync()
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
        }
    }
}
