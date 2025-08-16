using RabbitMQ.Client.Exceptions;
using RentalApi.Infrastructure.Messaging;

namespace RentalApi.Domain.Interfaces
{
    public interface IMessagePublisher
    {
        void Publish<T>(T message, string queueName);
    }
}