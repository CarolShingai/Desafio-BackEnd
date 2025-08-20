namespace RentalApi.Domain.Interfaces
{
    /// <summary>
    /// Interface for publishing messages to a message broker (e.g., RabbitMQ).
    /// Defines the contract for sending domain events and notifications.
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes a message synchronously to the specified queue.
        /// </summary>
        /// <typeparam name="T">Type of the message to publish.</typeparam>
        /// <param name="message">The message object to publish.</param>
        /// <param name="queueName">Name of the queue to publish the message to.</param>
        void Publish<T>(T message, string queueName);
        
        /// <summary>
        /// Publishes a message asynchronously to the specified queue.
        /// </summary>
        /// <typeparam name="T">Type of the message to publish.</typeparam>
        /// <param name="message">The message object to publish.</param>
        /// <param name="queueName">Name of the queue to publish the message to.</param>
        /// <param name="cancellationToken">Cancellation token for async operation.</param>
        /// <returns>A task representing the asynchronous publish operation.</returns>
        Task PublishAsync<T>(T message, string queueName,
                CancellationToken cancellationToken = default);
    }
}
