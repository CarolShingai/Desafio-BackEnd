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
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace RentalApi.Tests
{
    public class RabbitMqIntegrationTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer;
        private readonly RabbitMqContainer _rabbitMqContainer;
        private IServiceProvider? _serviceProvider;
        private IHost? _host;

        public RabbitMqIntegrationTests()
        {
            _postgresContainer = new PostgreSqlBuilder()
                .WithImage("postgres:16")
                .WithDatabase("testdb")
                .WithUsername("testuser")
                .WithPassword("testpass")
                .Build();

            _rabbitMqContainer = new RabbitMqBuilder()
                .WithImage("rabbitmq:3-management")
                .WithUsername("testuser")
                .WithPassword("testpass")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _postgresContainer.StartAsync();
            await _rabbitMqContainer.StartAsync();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _postgresContainer.GetConnectionString(),
                    ["RabbitMQ:Host"] = _rabbitMqContainer.Hostname,
                    ["RabbitMQ:Username"] = "testuser",
                    ["RabbitMQ:Password"] = "testpass"
                })
                .Build();

            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    
                    // Configurar Entity Framework com PostgreSQL
                    services.AddDbContext<RentalDbContext>(options =>
                        options.UseNpgsql(_postgresContainer.GetConnectionString()));
                    
                    // Registrar serviços
                    services.AddScoped<IMotoNotificationRepository, MotoNotificationRepository>();
                    services.AddScoped<IMessageConsumer, RabbitMqConsumer>();
                    services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
                    
                    // Mock do IMotoRepository se necessário para o background service
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

            // Criar o banco de dados e aplicar migrações
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RentalDbContext>();
            await context.Database.EnsureCreatedAsync();
            
            // Iniciar o host para que o background service funcione
            await _host.StartAsync();
        }

        [Fact]
        public async Task RabbitMqConsumer_ShouldInsertNotificationInDatabase_WhenMoto2024IsPublished()
        {
            // Arrange
            using var scope = _serviceProvider!.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<IMotoNotificationRepository>();
            
            var moto2024 = new Moto
            {
                Identifier = Guid.NewGuid().ToString(),
                Year = 2024,
                MotorcycleModel = "Honda CB 600F",
                LicensePlate = "ABC1234"
            };

            // Act - Publicar mensagem no RabbitMQ
            await publisher.PublishAsync(moto2024, "motoQueue");
            
            // Aguardar um pouco para o consumer processar a mensagem
            await Task.Delay(3000);

            // Assert - Verificar se a notificação foi inserida no banco
            var notifications = await notificationRepo.GetAllNotificationsAsync();
            
            Assert.NotEmpty(notifications);
            var notification = notifications.FirstOrDefault(n => n.MotorcycleId == moto2024.Id);
            Assert.NotNull(notification);
            Assert.Equal(moto2024.Id, notification.MotorcycleId);
            Assert.Contains("Moto registered successfully!", notification.Message);
        }

        [Fact]
        public async Task RabbitMqConsumer_ShouldNotInsertNotification_WhenMotoIsNot2024()
        {
            // Arrange
            using var scope = _serviceProvider!.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<IMotoNotificationRepository>();
            
            var moto2023 = new Moto
            {
                Identifier = Guid.NewGuid().ToString(),
                Year = 2023,
                MotorcycleModel = "Yamaha MT-07",
                LicensePlate = "XYZ5678"
            };

            // Contar notificações antes
            var notificationsBefore = await notificationRepo.GetAllNotificationsAsync();
            var countBefore = notificationsBefore.Count;

            // Act - Publicar mensagem no RabbitMQ
            await publisher.PublishAsync(moto2023, "motoQueue");
            
            // Aguardar um pouco para o consumer processar a mensagem
            await Task.Delay(2000);

            // Assert - Verificar que nenhuma nova notificação foi inserida
            var notificationsAfter = await notificationRepo.GetAllNotificationsAsync();
            var countAfter = notificationsAfter.Count;
            
            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public async Task RabbitMqConsumer_ShouldHandleMultipleMessages_Correctly()
        {
            // Arrange
            using var scope = _serviceProvider!.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<IMotoNotificationRepository>();
            
            var motos2024 = new[]
            {
                new Moto { Identifier = Guid.NewGuid().ToString(), Year = 2024, MotorcycleModel = "Honda CB 600F", LicensePlate = "ABC1111" },
                new Moto { Identifier = Guid.NewGuid().ToString(), Year = 2024, MotorcycleModel = "Yamaha MT-07", LicensePlate = "DEF2222" },
                new Moto { Identifier = Guid.NewGuid().ToString(), Year = 2023, MotorcycleModel = "Kawasaki Ninja", LicensePlate = "GHI3333" } // Esta não deve gerar notificação
            };

            // Act - Publicar múltiplas mensagens
            foreach (var moto in motos2024)
            {
                await publisher.PublishAsync(moto, "motoQueue");
            }
            
            // Aguardar processamento
            await Task.Delay(4000);

            // Assert - Verificar que apenas as motos 2024 geraram notificações
            var notifications = await notificationRepo.GetAllNotificationsAsync();
            var relevantNotifications = notifications.Where(n => 
                motos2024.Any(m => m.Id == n.MotorcycleId)).ToList();
            
            Assert.Equal(2, relevantNotifications.Count); // Apenas 2 motos de 2024
            
            foreach (var notification in relevantNotifications)
            {
                var correspondingMoto = motos2024.First(m => m.Id == notification.MotorcycleId);
                Assert.Equal(2024, correspondingMoto.Year);
            }
        }

        public async Task DisposeAsync()
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            
            await _postgresContainer.DisposeAsync();
            await _rabbitMqContainer.DisposeAsync();
        }
    }
}
