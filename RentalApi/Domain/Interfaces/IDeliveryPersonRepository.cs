using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for delivery person data access operations.
    /// Defines the contract for persisting and retrieving delivery person information.
    /// </summary>
    public interface IDeliveryPersonRepository
    {
        /// <summary>
        /// Adds a new delivery person to the repository.
        /// </summary>
        /// <param name="deliveryPerson">The delivery person entity to add.</param>
        /// <returns>The added delivery person entity with generated ID.</returns>
        Task<DeliveryPerson> AddDeliveryPersonAsync(DeliveryPerson deliveryPerson);
        
        /// <summary>
        /// Updates the CNH (driver's license) image for a delivery person.
        /// </summary>
        /// <param name="deliveryPersonId">Unique identifier of the delivery person.</param>
        /// <param name="image">Base64 encoded image or file path.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        Task<bool> AddCnhImageAsync(string deliveryPersonId, string image);
        
        /// <summary>
        /// Finds a delivery person by their unique business identifier.
        /// </summary>
        /// <param name="identifier">Unique business identifier of the delivery person.</param>
        /// <returns>The delivery person entity if found, otherwise null.</returns>
        Task<DeliveryPerson?> FindDeliveryPersonByIdentifierAsync(string identifier);
        
        /// <summary>
        /// Finds a delivery person by their CNH (driver's license) number.
        /// </summary>
        /// <param name="cnh">CNH number to search for.</param>
        /// <returns>The delivery person entity if found, otherwise null.</returns>
        Task<DeliveryPerson?> FindDeliveryPersonByCnhAsync(string cnh);
        
        /// <summary>
        /// Finds a delivery person by their CNPJ (Brazilian company registration number).
        /// </summary>
        /// <param name="cnpj">CNPJ to search for.</param>
        /// <returns>The delivery person entity if found, otherwise null.</returns>
        Task<DeliveryPerson?> FindDeliveryPersonByCnpjAsync(string cnpj);
        
        /// <summary>
        /// Validates if a CNPJ is unique and available for registration.
        /// </summary>
        /// <param name="cnpj">CNPJ to validate.</param>
        /// <returns>True if the CNPJ is available, false if already in use.</returns>
        Task<bool> ValidateCnpjAsync(string cnpj);
        
        /// <summary>
        /// Validates if a CNH number is unique and available for registration.
        /// </summary>
        /// <param name="cnh">CNH number to validate.</param>
        /// <returns>True if the CNH is available, false if already in use.</returns>
        Task<bool> ValidateCnhAsync(string cnh);
        
        /// <summary>
        /// Validates if the CNH type is acceptable for motorcycle rental.
        /// </summary>
        /// <param name="cnhType">CNH type to validate (must be 'A' or 'A+B').</param>
        /// <returns>True if the CNH type is valid for motorcycle rental, false otherwise.</returns>
        bool IsValidCnhType(string cnhType);
    }
}
