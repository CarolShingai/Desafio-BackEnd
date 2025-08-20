using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for motorcycle rental data access operations.
    /// Defines the contract for persisting and retrieving rental information.
    /// </summary>
    public interface IRentMotoRepository
    {
        /// <summary>
        /// Adds a new motorcycle rental to the repository.
        /// </summary>
        /// <param name="rentMoto">The rental entity to add.</param>
        /// <returns>The added rental entity with generated ID and timestamps.</returns>
        Task<RentMoto> AddRentalAsync(RentMoto rentMoto);
        
        /// <summary>
        /// Finds a rental by its unique business identifier.
        /// </summary>
        /// <param name="id">Unique business identifier of the rental.</param>
        /// <returns>The rental entity if found, otherwise null.</returns>
        Task<RentMoto?> FindRentalByIdAsync(string id);
        
        /// <summary>
        /// Updates an existing rental with new information (e.g., return date, final cost).
        /// </summary>
        /// <param name="rentMoto">The rental entity with updated information.</param>
        /// <returns>The updated rental entity.</returns>
        Task<RentMoto> UpdateRentAsync(RentMoto rentMoto);
    }
}
