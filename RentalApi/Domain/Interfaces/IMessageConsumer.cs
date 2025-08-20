namespace RentalApi.Domain.Interfaces
{
    /// <summary>
    /// Interface for consuming messages from a message broker (e.g., RabbitMQ).
    /// Defines the contract for receiving and processing domain events and notifications.
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Starts consuming messages from the specified queue asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of the message to consume.</typeparam>
        /// <param name="queueName">Name of the queue to consume messages from.</param>
        /// <param name="onMessageAsync">Async callback function to handle received messages.</param>
        /// <param name="cancellationToken">Cancellation token for stopping the consumption.</param>
        /// <returns>A task representing the asynchronous consumption operation.</returns>
        Task StartConsumingAsync<T>(string queueName, Func<T, Task> onMessageAsync, CancellationToken cancellationToken = default);
        
        // /// <summary>
        // /// Stops consuming messages from the specified queue.
        // /// </summary>
        // /// <param name="queueName">Name of the queue to stop consuming from.</param>
        // /// <returns>A task representing the asynchronous stop operation.</returns>
        // Task StopConsumingAsync(string queueName);
    }
}