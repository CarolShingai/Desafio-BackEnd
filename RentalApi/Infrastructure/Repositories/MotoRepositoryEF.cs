using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;

namespace RentalApi.Infrastructure.Repositories
{
    /// <summary>
    /// Entity Framework implementation of the IMotoRepository interface for motorcycle data access.
    /// </summary>
    public class MotoRepositoryEF : IMotoRepository
    {
        /// <summary>
        /// Database context for accessing motorcycle data.
        /// </summary>
        private readonly RentalDbContext _context;

        /// <summary>
        /// Initializes a new instance of the MotoRepositoryEF class.
        /// </summary>
        /// <param name="context">Database context for rental system.</param>
        public MotoRepositoryEF(RentalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all motorcycles from the database.
        /// </summary>
        public async Task<List<Moto>> FindMotoAllAsync()
        {
            return await _context.Motos.ToListAsync();
        }

        /// <summary>
        /// Finds a motorcycle by its unique identifier.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <returns>The Moto entity if found, otherwise null.</returns>
        public async Task<Moto?> FindByMotoIdentifierAsync(string identifier)
        {
            return await _context.Motos.FirstOrDefaultAsync(m => m.Identificador == identifier);
        }

        /// <summary>
        /// Finds a motorcycle by its license plate.
        /// </summary>
        /// <param name="license">License plate of the motorcycle.</param>
        /// <returns>The Moto entity if found, otherwise null.</returns>
        public async Task<Moto?> FindByMotoLicenseAsync(string license)
        {
            return await _context.Motos.FirstOrDefaultAsync(m => m.Placa == license);
        }

        /// <summary>
        /// Searches motorcycles by partial license plate.
        /// </summary>
        /// <param name="license">Partial or full license plate to search for.</param>
        /// <returns>List of Moto entities matching the search.</returns>
        public async Task<List<Moto>> SearchMotosByLicenseAsync(string license)
        {
            return await _context.Motos
                .Where(m => m.Placa.Contains(license)).ToListAsync();
        }

        /// <summary>
        /// Adds a new motorcycle to the database.
        /// </summary>
        /// <param name="moto">Moto entity to add.</param>
        /// <returns>The added Moto entity.</returns>
        public async Task<Moto> AddMotoAsync(Moto moto)
        {
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
            return moto;
        }

        /// <summary>
        /// Updates the license plate of a motorcycle.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <param name="license">New license plate.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public async Task<bool> UpdateMotoLicenseAsync(string identifier, string license)
        {
            var moto = await FindByMotoIdentifierAsync(identifier);
            if (moto == null) return false;

            moto.Placa = license;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Removes a motorcycle from the database by its identifier.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <returns>True if the removal was successful, otherwise false.</returns>
        public async Task<bool> RemoveMotoAsync(string identifier)
        {
            var moto = await FindByMotoIdentifierAsync(identifier);
            if (moto == null) return false;
            
            if (moto.IsRented)
                throw new Exception("Cannot delete a motorcycle that is currently rented.");
            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}