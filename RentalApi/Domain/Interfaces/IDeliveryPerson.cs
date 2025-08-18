using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IDeliveryPerson
    {
        Task<DeliveryPerson> AddDeliveryPersonAsync(DeliveryPerson deliveryPerson);
        Task<bool> AddCnhImageAsync(Guid deliveryPersonId, string base64Image);
        Task<DeliveryPerson?> FindDeliveryPersonByIdAsync(Guid id);
        Task<DeliveryPerson?> FindDeliveryPersonByCnhAsync(string cnh);
        Task<DeliveryPerson?> FindDeliveryPersonByCnpjAsync(string cnpj);
        Task<bool> ValidateCnpjAsync(string cnpj);
        Task<bool> ValidateCnhAsync(string cnh);
        bool IsValidCnhType(string cnhType);
    }
}