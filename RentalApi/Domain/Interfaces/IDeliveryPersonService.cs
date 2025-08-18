using RentalApi.Domain.Entities;

namespace RentalApi.Application.Services
{
    public interface IDeliveryPersonService
    {
        Task<DeliveryPerson> RegisterDeliveryPersonAsync(DeliveryPerson deliveryPerson);
        Task<bool> UpdateCnhImageAsync(Guid deliveryPersonId, string base64Image);
        Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(Guid id);
    }
}