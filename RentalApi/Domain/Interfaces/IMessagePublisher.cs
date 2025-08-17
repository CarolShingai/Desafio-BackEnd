namespace RentalApi.Domain.Interfaces
{
    public interface IMessagePublisher
    {
        void Publish<T>(T message, string queueName);
        Task PublishAsync<T>(T message, string queueName,
                CancellationToken cancellationToken = default);
    }
}
