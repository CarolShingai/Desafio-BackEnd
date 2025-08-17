using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using RentalApi.Domain.Interfaces;

// consumer container em diferente
namespace RentalApi.Infrastructure.Messaging
{
    public class RabbitMqConsumer : IMessageConsumer
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly Dictionary<string, string> _activeConsumers = new();

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

        public async Task StartConsumingAsync<T>(string queueName, Func<T, Task> onMessageAsync,
            CancellationToken cancellationToken = default)
        {
            if (_connection == null)
                _connection = await _factory.CreateConnectionAsync();
            if (_channel == null)
                _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            await _channel.BasicQosAsync(0, prefetchCount: 1, global: false);
            var consumer = new AsyncEventingBasicConsumer(_channel);

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
                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[RabbitMqConsumer] Erro ao processar mensagem: {ex.Message}\nStackTrace: {ex.StackTrace}");
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    }
            };
            var consumerTag = await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );
            _activeConsumers[queueName] = consumerTag;
        }
        // public async ValueTask DisposeAsync()
        // {
        //     if (_channel != null)
        //     {
        //         await _channel.CloseAsync();
        //         await _channel.DisposeAsync();
        //         _channel = null;
        //     }
        //     if (_connection != null)
        //     {
        //         await _connection.CloseAsync();
        //         await _connection.DisposeAsync();
        //         _connection = null;
        //     }
        // }
    }
} 
