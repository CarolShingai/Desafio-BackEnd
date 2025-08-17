using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RentalApi.Domain.Interfaces;
using RentalApi.Domain.Entities;

namespace RentalApi.Infrastructure.Background
{
    public class RabbitMqBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var messageConsumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
            var motoRepository = scope.ServiceProvider.GetRequiredService<IMotoRepository>();

            await messageConsumer.StartConsumingAsync<Moto>("motoQueue",
                async moto =>
                {
                    if (moto.Ano == 2024)
                    {
                        using var innerScope = _serviceProvider.CreateAsyncScope();
                        var notificationRepo = innerScope.ServiceProvider.GetRequiredService<IMotoNotificationRepository>();
                        var notification = new MotoNotification
                        {
                            MotorcycleId = moto.Identificador,
                            Message = "Moto registered successfully!",
                            NotifiedAt = DateTime.UtcNow
                        };
                        try
                        {
                            await notificationRepo.AddNotificationAsync(notification);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao adicionar notificação: {ex.Message}");
                        }
                    }
                }, stoppingToken);
        }
    }
}