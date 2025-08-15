using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for motorcycle data access operations.
    /// </summary>
    public interface IMotoRepository
    {
        /// <summary>
        /// Returns all motorcycles.
        /// </summary>
        Task<List<Moto>> FindMotoAllAsync();

        /// <summary>
        /// Finds a motorcycle by its unique identifier.
        /// </summary>
        Task<Moto?> FindByMotoIdentifierAsync(string identifier);

        /// <summary>
        /// Finds a motorcycle by its license plate.
        /// </summary>
        Task<Moto?> FindByMotoLicenseAsync(string license);

        /// <summary>
        /// Searches motorcycles by partial license plate.
        /// </summary>
        Task<List<Moto>> SearchMotosByLicenseAsync(string license);

        /// <summary>
        /// Adds a new motorcycle.
        /// </summary>
        Task<Moto> AddMotoAsync(Moto moto);

        /// <summary>
        /// Updates the license plate of a motorcycle.
        /// </summary>
        Task<bool> UpdateMotoLicenseAsync(string identifier, string license);

        /// <summary>
        /// Removes a motorcycle by its identifier.
        /// </summary>
        Task<bool> RemoveMotoAsync(string identifier);
    }
}