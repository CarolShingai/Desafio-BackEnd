using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Application.Services
{
    public class DeliveryPersonService : IDeliveryPersonService
    {
        private readonly IDeliveryPerson _deliveryPersonRepository;

        public DeliveryPersonService(IDeliveryPerson deliveryPersonRepository)
        {
            _deliveryPersonRepository = deliveryPersonRepository;
        }

        public async Task<DeliveryPerson> RegisterDeliveryPersonAsync(DeliveryPerson deliveryPerson)
        {
            return await _deliveryPersonRepository.AddDeliveryPersonAsync(deliveryPerson);
        }

        public async Task<bool> UpdateCnhImageAsync(Guid deliveryPersonId, string base64Image)
        {
            return await _deliveryPersonRepository.AddCnhImageAsync(deliveryPersonId, base64Image);
        }
        public async Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(Guid id)
        {
            return await _deliveryPersonRepository.FindDeliveryPersonByIdAsync(id);
        }
    }
}