using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Infrastructure.Messaging
{
    /// <summary>
    /// Implementation of <see cref="IMessagePublisher"/> for publishing messages to RabbitMQ.
    /// Provides both synchronous and asynchronous methods for message publishing.
    /// </summary>
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly ConnectionFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqPublisher"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration containing RabbitMQ connection settings.</param>
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

        /// <summary>
        /// Publishes a message synchronously to the specified queue.
        /// </summary>
        /// <typeparam name="T">Type of the message to publish.</typeparam>
        /// <param name="message">The message object to publish.</param>
        /// <param name="queueName">Name of the queue to publish the message to.</param>
        public void Publish<T>(T message, string queueName)
        {
            PublishAsync(message, queueName, CancellationToken.None).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Publishes a message asynchronously to the specified queue.
        /// Creates a durable queue if it doesn't exist and sends the message with persistence enabled.
        /// </summary>
        /// <typeparam name="T">Type of the message to publish.</typeparam>
        /// <param name="message">The message object to publish.</param>
        /// <param name="queueName">Name of the queue to publish the message to.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
        /// <exception cref="ArgumentException">Thrown when queueName is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when publishing fails.</exception>
        public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name is required", nameof(queueName));

            try
            {
                using var connection = await _factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                // Declare a durable queue
                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Serialize message to JSON
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                // Set message properties for persistence
                var properties = new BasicProperties
                {
                    Persistent = true
                };

                // Publish the message
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
