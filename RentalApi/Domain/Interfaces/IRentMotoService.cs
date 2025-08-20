using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    /// <summary>
    /// Interface for motorcycle rental service operations.
    /// Defines the contract for business logic related to rental management.
    /// </summary>
    public interface IRentMotoService
    {
        /// <summary>
        /// Creates a new motorcycle rental.
        /// </summary>
        /// <param name="deliveryPersonId">Unique identifier of the delivery person.</param>
        /// <param name="motoId">Unique identifier of the motorcycle.</param>
        /// <param name="planDays">Number of days for the rental plan (7, 15, 30, 45, or 50).</param>
        /// <returns>The created RentMoto entity.</returns>
        Task<RentMoto> CreateRentalAsync(string deliveryPersonId, string motoId, int planDays);
        
        /// <summary>
        /// Informs the actual return date of a rental and calculates the final cost.
        /// </summary>
        /// <param name="rentId">Unique identifier of the rental.</param>
        /// <param name="actualReturnDate">Actual date when the motorcycle was returned.</param>
        /// <returns>The updated RentMoto entity with calculated final cost.</returns>
        Task<RentMoto> InformReturnDateAsync(string rentId, DateTime actualReturnDate);
        
        /// <summary>
        /// Retrieves a rental by its unique identifier.
        /// </summary>
        /// <param name="rentId">Unique identifier of the rental.</param>
        /// <returns>The RentMoto entity if found, otherwise null.</returns>
        Task<RentMoto?> GetRentalByIdAsync(string rentId);
        
        /// <summary>
        /// Gets the final calculated value of a rental after return.
        /// </summary>
        /// <param name="rentId">Unique identifier of the rental.</param>
        /// <returns>The final calculated rental value including any penalties or discounts.</returns>
        Task<decimal> GetFinalRentalValueAsync(string rentId);
        
        /// <summary>
        /// Simulates the rental value calculation for a given return date without updating the rental.
        /// </summary>
        /// <param name="rentId">Unique identifier of the rental.</param>
        /// <param name="ReturnDate">Proposed return date for simulation.</param>
        /// <returns>The calculated rental value for the proposed return date.</returns>
        Task<decimal> SimulateReturnValueAsync(string rentId, DateTime ReturnDate);
    }
}
