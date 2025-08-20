using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;

namespace RentalApi.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing motorcycle notification operations in the database.
    /// Provides data access methods for creating and retrieving notification records.
    /// </summary>
    public class MotoNotificationRepository : IMotoNotificationRepository
    {
        private readonly RentalDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotoNotificationRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for notification operations.</param>
        public MotoNotificationRepository(RentalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new motorcycle notification to the database asynchronously.
        /// </summary>
        /// <param name="notification">The notification entity to add to the database.</param>
        /// <returns>A task representing the asynchronous operation with the added notification entity.</returns>
        public async Task<MotoNotification> AddNotificationAsync(MotoNotification notification)
        {
            await _context.MotoNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        /// <summary>
        /// Retrieves all motorcycle notifications from the database asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with a list of all notification entities.</returns>
        public async Task<List<MotoNotification>> GetAllNotificationsAsync()
        {
            return await _context.MotoNotifications.ToListAsync();
        }
    }
}