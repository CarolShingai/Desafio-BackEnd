namespace RentalApi.Domain.Interfaces
{
    public interface IMessageConsumer
    {
        Task StartConsumingAsync<T>(string queueName, Func<T, Task> onMessageAsync, CancellationToken cancellationToken = default);
        // Task StopConsumingAsync(string queueName);
    }
}