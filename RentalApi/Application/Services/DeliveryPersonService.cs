using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Application.DTOs;

namespace RentalApi.Application.Services
{
    /// <summary>
    /// Service class for business logic related to delivery person operations.
    /// </summary>
    public class DeliveryPersonService : IDeliveryPersonService
    {
        private readonly IDeliveryPersonRepository _deliveryPersonRepository;

        /// <summary>
        /// Initializes a new instance of the DeliveryPersonService class.
        /// </summary>
        /// <param name="deliveryPersonRepository">Repository for delivery person operations.</param>
        public DeliveryPersonService(IDeliveryPersonRepository deliveryPersonRepository)
        {
            _deliveryPersonRepository = deliveryPersonRepository;
        }

        /// <summary>
        /// Registers a new delivery person in the system.
        /// </summary>
        /// <param name="dto">Delivery person registration data.</param>
        /// <returns>The registered DeliveryPerson entity.</returns>
        public async Task<DeliveryPerson> RegisterDeliveryPersonAsync(CreateDeliveryPersonRequest dto)
        {
            var deliveryPerson = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Identifier = dto.Identifier,
                Name = dto.Name,
                Cnpj = dto.Cnpj,
                BirthDate = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Utc),
                Cnh = dto.CnhNumber,
                CnhType = dto.CnhType,
                CnhImage = dto.CnhImage
            };
            return await _deliveryPersonRepository.AddDeliveryPersonAsync(deliveryPerson);
        }

        /// <summary>
        /// Updates the driver's license (CNH) image for a delivery person.
        /// </summary>
        /// <param name="deliveryPersonId">Unique identifier of the delivery person.</param>
        /// <param name="base64Image">Base64 encoded image of the new CNH.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public async Task<bool> UpdateCnhImageAsync(string deliveryPersonId, string base64Image)
        {
            return await _deliveryPersonRepository.AddCnhImageAsync(deliveryPersonId, base64Image);
        }
        
        /// <summary>
        /// Retrieves a delivery person by their unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the delivery person.</param>
        /// <returns>The DeliveryPerson entity if found, otherwise null.</returns>
        public async Task<DeliveryPerson?> GetDeliveryPersonByIdentifierAsync(string id)
        {
            return await _deliveryPersonRepository.FindDeliveryPersonByIdentifierAsync(id);
        }
    }
}