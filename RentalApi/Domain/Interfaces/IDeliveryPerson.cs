using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IDeliveryPerson
    {
        Task<DeliveryPerson> AddDeliveryPersonAsync(DeliveryPerson deliveryPerson);
        // Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(Guid id);
        Task<DeliveryPerson?> GetDeliveryPersonByCnpjAsync(string cnpj);
        Task<DeliveryPerson?> GetDeliveryPersonByCnhAsync(string cnh);
        Task<bool> ValidateCnpjAsync(string cnpj);
        Task<bool> ValidateCnhAsync(string cnh);
        bool IsValidCnhType(string cnhType);
    }
}