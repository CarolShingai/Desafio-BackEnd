using RentalApi.Domain.Entities;
using RentalApi.Application.DTOs;

namespace RentalApi.Application.Services
{
    public interface IDeliveryPersonService
    {
        Task<DeliveryPerson> RegisterDeliveryPersonAsync(CreateDeliveryPersonRequest dto);
        Task<bool> UpdateCnhImageAsync(string deliveryPersonId, string base64Image);
        Task<DeliveryPerson?> GetDeliveryPersonByIdentifierAsync(string identifier);
    }
}