using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IDeliveryPersonRepository
    {
        Task<DeliveryPerson> AddDeliveryPersonAsync(DeliveryPerson deliveryPerson);
        Task<bool> AddCnhImageAsync(string deliveryPersonId, string image);
        Task<DeliveryPerson?> FindDeliveryPersonByIdentifierAsync(string identifier);
        Task<DeliveryPerson?> FindDeliveryPersonByCnhAsync(string cnh);
        Task<DeliveryPerson?> FindDeliveryPersonByCnpjAsync(string cnpj);
        Task<bool> ValidateCnpjAsync(string cnpj);
        Task<bool> ValidateCnhAsync(string cnh);
        bool IsValidCnhType(string cnhType);
    }
}
