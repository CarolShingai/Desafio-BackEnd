using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IDeliveryPerson
    {
        Task<DeliveryPerson> AddDeliveryPersonAsync(DeliveryPerson deliveryPerson);
        // Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(Guid id);
        Task<bool> GetDeliveryPersonByCnpjAsync(string cnpj);
        Task<bool> GetDeliveryPersonByCnhAsync(string cnh);
        Task<bool> ExistsCnpjAsync(string cnpj);
        Task<bool> ExistsCnhAsync(string cnh);
        Task<bool> IsValidCnhTypeAsync(string cnhType);
    }
}