using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher(IConfiguration configuration)
        {
            var hostName = configuration["RabbitMQ:Host"] ?? "localhost";
            var userName = configuration["RabbitMQ:Username"] ?? "guest";
            var password = configuration["RabbitMQ:Password"] ?? "guest";

            _factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };
        }

        public void Publish<T>(T message, string queueName)
        {
            PublishAsync(message, queueName, CancellationToken.None).GetAwaiter().GetResult();
        }

        public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name is required", nameof(queueName));

            try
            {
                using var connection = await _factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    Persistent = true
                };

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    body: body,
                    basicProperties: properties,
                    mandatory: true
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to publish message to queue '{queueName}': {ex.Message}", ex);
            }
        }
    }
}
