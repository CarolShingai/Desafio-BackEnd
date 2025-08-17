using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;

namespace RentalApi.Infrastructure.Repositories
{
    /// <summary>
    /// Entity Framework implementation of the IMotoRepository2024 interface for motorcycle data access.
    /// </summary>
    public class MotoNotificationRepository : IMotoNotificationRepository
    {
        private readonly RentalDbContext _context;

        public MotoNotificationRepository(RentalDbContext context)
        {
            _context = context;
        }

        public async Task<MotoNotification> AddNotificationAsync(MotoNotification notification)
        {
            await _context.MotoNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<List<MotoNotification>> GetAllNotificationsAsync()
        {
            return await _context.MotoNotifications.ToListAsync();
        }
    }
}