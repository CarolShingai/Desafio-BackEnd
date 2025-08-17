using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IMotoNotificationRepository
    {
        Task<MotoNotification> AddNotificationAsync(MotoNotification notification);
        Task<List<MotoNotification>> GetAllNotificationsAsync();
    }
}