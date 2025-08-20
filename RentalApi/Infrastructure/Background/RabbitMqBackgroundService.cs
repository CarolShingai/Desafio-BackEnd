using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RentalApi.Domain.Interfaces;
using RentalApi.Domain.Entities;

namespace RentalApi.Infrastructure.Background
{
    /// <summary>
    /// Background service responsible for consuming messages from RabbitMQ and processing motorcycle registration events.
    /// Listens to the 'motoQueue' and creates notifications for motorcycles registered in 2024.
    /// </summary>
    public class RabbitMqBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqBackgroundService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider for dependency resolution.</param>
        public RabbitMqBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Executes the background service, consuming messages from RabbitMQ and processing motorcycle registration events.
        /// </summary>
        /// <param name="stoppingToken">Token to signal cancellation of the background task.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var messageConsumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
            var motoRepository = scope.ServiceProvider.GetRequiredService<IMotoRepository>();

            await messageConsumer.StartConsumingAsync<Moto>("motoQueue",
                async moto =>
                {
                    // Only process motorcycles registered in 2024
                    if (moto.Year == 2024)
                    {
                        using var innerScope = _serviceProvider.CreateAsyncScope();
                        var notificationRepo = innerScope.ServiceProvider.GetRequiredService<IMotoNotificationRepository>();
                        var notification = new MotoNotification
                        {
                            MotorcycleId = moto.Id,
                            Message = "Moto registered successfully!",
                            NotifiedAt = DateTime.UtcNow
                        };
                        try
                        {
                            await notificationRepo.AddNotificationAsync(notification);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error adding notification: {ex.Message}");
                        }
                    }
                }, stoppingToken);
        }
    }
}