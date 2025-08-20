using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;

namespace RentalApi.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing motorcycle rental operations in the database.
    /// Provides data access methods for creating, retrieving, and updating rental records.
    /// </summary>
    public class RentMotoRepository : IRentMotoRepository
    {
        private readonly RentalDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentMotoRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for rental operations.</param>
        public RentMotoRepository(RentalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new motorcycle rental record to the database asynchronously.
        /// </summary>
        /// <param name="rentMoto">The rental entity to add to the database.</param>
        /// <returns>A task representing the asynchronous operation with the added rental entity.</returns>
        public async Task<RentMoto> AddRentalAsync(RentMoto rentMoto)
        {
            _context.RentMotos.Add(rentMoto);
            await _context.SaveChangesAsync();
            return rentMoto;
        }

        /// <summary>
        /// Retrieves a motorcycle rental record by its unique identifier asynchronously.
        /// Includes related motorcycle and delivery person data in the result.
        /// </summary>
        /// <param name="id">The unique identifier of the rental to retrieve.</param>
        /// <returns>
        /// A task representing the asynchronous operation with the rental entity if found, 
        /// or null if no rental with the specified ID exists.
        /// </returns>
        public async Task<RentMoto?> FindRentalByIdAsync(string id)
        {
            return await _context.RentMotos
            .Include(r => r.Moto)
            .Include(r => r.DeliveryPerson)
            .FirstOrDefaultAsync(r => r.RentId == id);
        }

        /// <summary>
        /// Updates an existing motorcycle rental record in the database asynchronously.
        /// </summary>
        /// <param name="rentMoto">The rental entity with updated information.</param>
        /// <returns>A task representing the asynchronous operation with the updated rental entity.</returns>
        public async Task<RentMoto> UpdateRentAsync(RentMoto rentMoto)
        {
            _context.RentMotos.Update(rentMoto);
            await _context.SaveChangesAsync();
            return rentMoto;
        }
    }
}