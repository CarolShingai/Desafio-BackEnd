using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using RentalApi.Domain.Interfaces;

// consumer container em diferente
namespace RentalApi.Infrastructure.Messaging
{
    /// <summary>
    /// Implementation of <see cref="IMessageConsumer"/> for consuming messages from RabbitMQ.
    /// Manages message consumption with automatic recovery and proper acknowledgments.
    /// </summary>
    public class RabbitMqConsumer : IMessageConsumer
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly Dictionary<string, string> _activeConsumers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConsumer"/> class.
        /// Configures connection factory with automatic recovery capabilities.
        /// </summary>
        /// <param name="configuration">Application configuration containing RabbitMQ connection settings.</param>
        public RabbitMqConsumer(IConfiguration configuration)
        {
            var hostName = configuration["RabbitMQ:Host"] ?? "localhost";
            var userName = configuration["RabbitMQ:Username"] ?? "guest";
            var password = configuration["RabbitMQ:Password"] ?? "guest";

            _factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
        }

        /// <summary>
        /// Starts consuming messages from the specified queue asynchronously.
        /// Creates a durable queue if it doesn't exist and sets up message handling with acknowledgments.
        /// </summary>
        /// <typeparam name="T">Type of message to consume and deserialize.</typeparam>
        /// <param name="queueName">Name of the queue to consume messages from.</param>
        /// <param name="onMessageAsync">Asynchronous callback function to handle received messages.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Messages are acknowledged (ACK) on successful processing or negative acknowledged (NACK) 
        /// without requeue on processing errors. Uses manual acknowledgment mode for reliability.
        /// </remarks>
        public async Task StartConsumingAsync<T>(string queueName, Func<T, Task> onMessageAsync,
            CancellationToken cancellationToken = default)
        {
            if (_connection == null)
                _connection = await _factory.CreateConnectionAsync();
            if (_channel == null)
                _channel = await _connection.CreateChannelAsync();
            
            // Declare a durable queue to ensure persistence
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            
            // Set QoS to process one message at a time for better distribution
            await _channel.BasicQosAsync(0, prefetchCount: 1, global: false);
            var consumer = new AsyncEventingBasicConsumer(_channel);

            // Setup message handling with proper error handling and acknowledgments
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[RabbitMqConsumer] Mensagem recebida: {json}");
                    
                    var message = JsonSerializer.Deserialize<T>(json);
                    if (message == null)
                    {
                        Console.WriteLine("[RabbitMqConsumer] Falha na desserialização. Tipo esperado: " + typeof(T).Name);
                    }
                    else
                    {
                        Console.WriteLine($"[RabbitMqConsumer] Mensagem desserializada com sucesso. Tipo: {typeof(T).Name}");
                        await onMessageAsync(message);
                    }
                    
                    // Acknowledge successful processing
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMqConsumer] Erro ao processar mensagem: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    // Negative acknowledge without requeue on error
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };
            
            // Start consuming with manual acknowledgment
            var consumerTag = await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );
            
            // Track active consumers for management
            _activeConsumers[queueName] = consumerTag;
        }
    }
} 
