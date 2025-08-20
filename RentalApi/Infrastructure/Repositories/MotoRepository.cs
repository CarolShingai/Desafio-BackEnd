using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory repository implementation for motorcycle data access operations (for testing or legacy use).
    /// Provides CRUD operations for motorcycles using an in-memory collection.
    /// </summary>
    public class MotoRepository : IMotoRepository
    {
        private readonly List<Moto> _motos = new List<Moto>();
        private int _nextId = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotoRepository"/> class with sample data.
        /// </summary>
        public MotoRepository()
        {
            _motos.Add(new Moto
            {
                Id = 1,
                Year = 2023,
                MotorcycleModel = "XL200",
                LicensePlate = "ABC-1272",
                Identifier = "Moto-001",
                IsRented = false
            });
        }

        /// <summary>
        /// Retrieves all motorcycles from the in-memory collection asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with a list of all motorcycles.</returns>
        public async Task<List<Moto>> FindMotoAllAsync()
        {
            return await Task.FromResult(_motos);
        }

        /// <summary>
        /// Finds a motorcycle by its unique identifier asynchronously.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <returns>
        /// A task representing the asynchronous operation with the motorcycle entity if found,
        /// or null if no motorcycle with the specified identifier exists.
        /// </returns>
        public async Task<Moto?> FindByMotoIdentifierAsync(string identifier)
        {
            var moto = _motos.FirstOrDefault(m => m.Identifier == identifier);
            return await Task.FromResult(moto);
        }

        /// <summary>
        /// Finds a motorcycle by its license plate asynchronously.
        /// </summary>
        /// <param name="license">License plate of the motorcycle to find.</param>
        /// <returns>
        /// A task representing the asynchronous operation with the motorcycle entity if found,
        /// or null if no motorcycle with the specified license plate exists.
        /// </returns>
        public async Task<Moto?> FindByMotoLicenseAsync(string license)
        {
            var moto = _motos.FirstOrDefault(m => m.LicensePlate == license);
            return await Task.FromResult(moto);
        }

        /// <summary>
        /// Searches for motorcycles by partial license plate match asynchronously.
        /// </summary>
        /// <param name="license">Partial or complete license plate to search for.</param>
        /// <returns>A task representing the asynchronous operation with a list of matching motorcycles.</returns>
        public async Task<List<Moto>> SearchMotosByLicenseAsync(string license)
        {
            var result = _motos.Where(m => m.LicensePlate.Contains(license)).ToList();
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Adds a new motorcycle to the in-memory collection asynchronously.
        /// </summary>
        /// <param name="moto">The motorcycle entity to add.</param>
        /// <returns>A task representing the asynchronous operation with the added motorcycle entity.</returns>
        public async Task<Moto> AddMotoAsync(Moto moto)
        {
            moto.Id = _nextId++;
            _motos.Add(moto);
            return await Task.FromResult(moto);
        }

        /// <summary>
        /// Updates the license plate of an existing motorcycle asynchronously.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle to update.</param>
        /// <param name="license">New license plate value.</param>
        /// <returns>
        /// A task representing the asynchronous operation with true if the update was successful,
        /// false if the motorcycle was not found or the license plate already exists.
        /// </returns>
        public async Task<bool> UpdateMotoLicenseAsync(string identifier, string license)
        {
            var moto = await FindByMotoIdentifierAsync(identifier);
            if (moto == null) return false;

            var equalLicense = await SearchMotosByLicenseAsync(license);
            if (equalLicense != null) return false;

            moto.LicensePlate = license;
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Removes a motorcycle from the repository by its identifier asynchronously.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle to remove.</param>
        /// <returns>
        /// A task representing the asynchronous operation with true if the removal was successful,
        /// false if the motorcycle was not found.
        /// </returns>
        /// <exception cref="Exception">Thrown when attempting to delete a motorcycle that is currently rented.</exception>
        public async Task<bool> RemoveMotoAsync(string identifier)
        {
            var moto = await FindByMotoIdentifierAsync(identifier);
            if (moto == null)
                return await Task.FromResult(false);

            if (moto.IsRented)
                throw new Exception("Cannot delete a motorcycle that is currently rented.");
            _motos.Remove(moto);
            return await Task.FromResult(true);
        }
    }
}