using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;
using RentalApi.Infrastructure.Messaging;
using RentalApi.Infrastructure.Repositories;
using System.Text.Json;
using Xunit;

namespace RentalApi.Tests
{
    public class RabbitMqManualIntegrationTests : IAsyncLifetime
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
                        options.UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}"));
                    
                    // Registrar serviços
                    services.AddScoped<IMotoNotificationRepository, MotoNotificationRepository>();
                    services.AddScoped<IMessageConsumer, RabbitMqConsumer>();
                    services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
                });

            _host = hostBuilder.Build();
            _serviceProvider = _host.Services;

            // Criar o banco de dados
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RentalDbContext>();
            await context.Database.EnsureCreatedAsync();
        }

        [Fact]
        public async Task RabbitMqConsumer_ShouldProcessMoto2024_AndSaveNotificationManually()
        {
            // Arrange
            using var scope = _serviceProvider!.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<IMotoNotificationRepository>();
            
            var moto2024 = new Moto
            {
                Identificador = Guid.NewGuid().ToString(),
                Ano = 2024,
                Modelo = "Honda CB 600F",
                Placa = "ABC1234"
            };

            var processedCount = 0;
            var processedMoto = new TaskCompletionSource<Moto>();

            // Act - Configurar consumer manual para processar mensagens
            await consumer.StartConsumingAsync<Moto>("testQueue",
                async moto =>
                {
                    processedCount++;
                    if (moto.Ano == 2024)
                    {
                        // Usar o mesmo scope para garantir que estamos usando o mesmo contexto de banco
                        var notification = new MotoNotification(
                            moto.Identificador,
                            moto.Ano,
                            moto.Modelo,
                            moto.Placa
                        );
                        await notificationRepo.AddNotificationAsync(notification);
                        processedMoto.SetResult(moto);
                    }
                });

            // Publicar a mensagem
            var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            await publisher.PublishAsync(moto2024, "testQueue");

            // Aguardar o processamento
            var timeout = Task.Delay(5000);
            var completedTask = await Task.WhenAny(processedMoto.Task, timeout);
            
            // Assert
            Assert.Equal(processedMoto.Task, completedTask); // Verifica se não foi timeout
            Assert.True(processedCount > 0);
            
            var notifications = await notificationRepo.GetAllNotificationsAsync();
            Assert.NotEmpty(notifications);
            
            var notification = notifications.FirstOrDefault(n => n.MotorcycleId == moto2024.Identificador);
            Assert.NotNull(notification);
            Assert.Equal(moto2024.Identificador, notification.MotorcycleId);
            Assert.Equal(2024, notification.Year);
            Assert.Equal(moto2024.Modelo, notification.Model);
            Assert.Equal(moto2024.Placa, notification.LicensePlate);
        }

        [Fact]
        public async Task RabbitMqConsumer_ShouldIgnoreNon2024Motos()
        {
            // Arrange
            using var scope = _serviceProvider!.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<IMotoNotificationRepository>();
            
            var moto2023 = new Moto
            {
                Identificador = Guid.NewGuid().ToString(),
                Ano = 2023,
                Modelo = "Yamaha MT-07",
                Placa = "XYZ5678"
            };

            var processedCount = 0;
            var processedAny = new TaskCompletionSource<bool>();

            // Act
            await consumer.StartConsumingAsync<Moto>("testQueue2",
                async moto =>
                {
                    processedCount++;
                    if (moto.Ano == 2024)
                    {
                        // Usar o mesmo scope
                        var notification = new MotoNotification(
                            moto.Identificador,
                            moto.Ano,
                            moto.Modelo,
                            moto.Placa
                        );
                        await notificationRepo.AddNotificationAsync(notification);
                    }
                    processedAny.SetResult(true);
                });

            var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            await publisher.PublishAsync(moto2023, "testQueue2");

            // Aguardar o processamento
            var timeout = Task.Delay(3000);
            var completedTask = await Task.WhenAny(processedAny.Task, timeout);
            
            // Assert - Verificar que a mensagem foi processada mas nenhuma notificação foi salva
            Assert.Equal(processedAny.Task, completedTask);
            Assert.True(processedCount > 0);
            
            var notifications = await notificationRepo.GetAllNotificationsAsync();
            var moto2023Notifications = notifications.Where(n => n.MotorcycleId == moto2023.Identificador);
            Assert.Empty(moto2023Notifications); // Não deve ter notificações para motos não-2024
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
