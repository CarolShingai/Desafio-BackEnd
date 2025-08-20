using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for motorcycle notification data access operations.
    /// Defines the contract for persisting and retrieving notification events.
    /// </summary>
    public interface IMotoNotificationRepository
    {
        /// <summary>
        /// Adds a new motorcycle notification to the repository.
        /// </summary>
        /// <param name="notification">The notification entity to add.</param>
        /// <returns>The added notification entity with generated ID and timestamp.</returns>
        Task<MotoNotification> AddNotificationAsync(MotoNotification notification);
        
        /// <summary>
        /// Retrieves all motorcycle notifications from the repository.
        /// </summary>
        /// <returns>A list of all notification entities ordered by creation date.</returns>
        Task<List<MotoNotification>> GetAllNotificationsAsync();
    }
}