using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Application.DTOs;

namespace RentalApi.Application.Services
{
    public class DeliveryPersonService : IDeliveryPersonService
    {
        private readonly IDeliveryPersonRepository _deliveryPersonRepository;

        public DeliveryPersonService(IDeliveryPersonRepository deliveryPersonRepository)
        {
            _deliveryPersonRepository = deliveryPersonRepository;
        }

        public async Task<DeliveryPerson> RegisterDeliveryPersonAsync(CreateDeliveryPersonRequest dto)
        {
            var deliveryPerson = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Identifier = dto.Identifier,
                Name = dto.Name,
                Cnpj = dto.Cnpj,
                BirthDate = dto.BirthDate,
                Cnh = dto.CnhNumber,
                CnhType = dto.CnhType,
                CnhImage = dto.CnhImage
            };
            return await _deliveryPersonRepository.AddDeliveryPersonAsync(deliveryPerson);
        }

        public async Task<bool> UpdateCnhImageAsync(string deliveryPersonId, string base64Image)
        {
            return await _deliveryPersonRepository.AddCnhImageAsync(deliveryPersonId, base64Image);
        }
        public async Task<DeliveryPerson?> GetDeliveryPersonByIdentifierAsync(string id)
        {
            return await _deliveryPersonRepository.FindDeliveryPersonByIdentifierAsync(id);
        }
    }
}