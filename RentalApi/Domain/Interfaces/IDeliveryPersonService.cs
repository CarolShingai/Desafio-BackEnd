using RentalApi.Domain.Entities;
using RentalApi.Application.DTOs;

namespace RentalApi.Application.Services
{
    /// <summary>
    /// Interface for delivery person service operations.
    /// Defines the contract for business logic related to delivery person management.
    /// </summary>
    public interface IDeliveryPersonService
    {
        /// <summary>
        /// Registers a new delivery person in the system.
        /// </summary>
        /// <param name="dto">Delivery person registration data.</param>
        /// <returns>The registered DeliveryPerson entity.</returns>
        Task<DeliveryPerson> RegisterDeliveryPersonAsync(CreateDeliveryPersonRequest dto);
        
        /// <summary>
        /// Updates the driver's license (CNH) image for a delivery person.
        /// </summary>
        /// <param name="deliveryPersonId">Unique identifier of the delivery person.</param>
        /// <param name="base64Image">Base64 encoded image of the new CNH.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        Task<bool> UpdateCnhImageAsync(string deliveryPersonId, string base64Image);
        
        /// <summary>
        /// Retrieves a delivery person by their unique identifier.
        /// </summary>
        /// <param name="identifier">Unique identifier of the delivery person.</param>
        /// <returns>The DeliveryPerson entity if found, otherwise null.</returns>
        Task<DeliveryPerson?> GetDeliveryPersonByIdentifierAsync(string identifier);
    }
}