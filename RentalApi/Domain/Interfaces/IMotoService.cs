using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    /// <summary>
    /// Interface for motorcycle service operations.
    /// Defines the contract for business logic related to motorcycle management.
    /// </summary>
    public interface IMotoService
    {
        /// <summary>
        /// Retrieves all motorcycles from the system.
        /// </summary>
        /// <returns>A list of all Moto entities.</returns>
        Task<List<Moto>> GetAllMoto();
        
        /// <summary>
        /// Retrieves a motorcycle by its unique identifier.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <returns>The Moto entity if found, otherwise null.</returns>
        Task<Moto?> GetMotoByIdentifierAsync(string identifier);
        
        /// <summary>
        /// Registers a new motorcycle in the system.
        /// </summary>
        /// <param name="moto">Moto entity to register.</param>
        /// <returns>The registered Moto entity.</returns>
        Task<Moto> RegisterNewMotoAsync(Moto moto);
        
        /// <summary>
        /// Updates the license plate of an existing motorcycle.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <param name="license">New license plate to assign.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        Task<bool> ChangeMotoLicenseAsync(string identifier, string license);
        
        /// <summary>
        /// Deletes a registered motorcycle from the system.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle to delete.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        Task<bool> DeleteRegisteredMotoAsync(string identifier);
    }
}