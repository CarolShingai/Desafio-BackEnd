using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory repository implementation for motorcycle data access operations (for testing or legacy use).
    /// </summary>
    public class MotoRepository : IMotoRepository
    {
        private readonly List<Moto> _motos = new List<Moto>();
        private int _nextId = 1;

        /// <summary>
        /// Initializes the repository with a default motorcycle.
        /// </summary>
        public MotoRepository()
        {
            _motos.Add(new Moto
            {
                Id = 1,
                Ano = 2023,
                Modelo = "XL200",
                Placa = "ABC-1272"
            });
        }
        /// <summary>
        /// Returns all motorcycles in the repository.
        /// </summary>
        public async Task<List<Moto>> FindMotoAllAsync()
        {
            return await Task.FromResult(_motos);
        }
        /// <summary>
        /// Finds a motorcycle by its unique identifier.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <returns>The Moto entity if found, otherwise null.</returns>
        public async Task<Moto?> FindByMotoIdentifierAsync(string identifier)
        {
            var moto = _motos.FirstOrDefault(m => m.Identificador == identifier);
            return await Task.FromResult(moto);
        }
        /// <summary>
        /// Finds a motorcycle by its license plate.
        /// </summary>
        /// <param name="license">License plate of the motorcycle.</param>
        /// <returns>The Moto entity if found, otherwise null.</returns>
        public async Task<Moto?> FindByMotoLicenseAsync(string license)
        {
            var moto = _motos.FirstOrDefault(m => m.Placa == license);
            return await Task.FromResult(moto);
        }
        /// <summary>
        /// Searches motorcycles by partial license plate.
        /// </summary>
        /// <param name="license">Partial or full license plate to search for.</param>
        /// <returns>List of Moto entities matching the search.</returns>
        public async Task<List<Moto>> SearchMotosByLicenseAsync(string license)
        {
            var result = _motos.Where(m => m.Placa.Contains(license)).ToList();
            return await Task.FromResult(result);
        }
        /// <summary>
        /// Adds a new motorcycle to the repository.
        /// </summary>
        /// <param name="moto">Moto entity to add.</param>
        /// <returns>The added Moto entity.</returns>
        public async Task<Moto> AddMotoAsync(Moto moto)
        {
            moto.Id = _nextId++;
            _motos.Add(moto);
            return await Task.FromResult(moto);
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

            var equalLicense = await SearchMotosByLicenseAsync(license);
            if (equalLicense != null) return false;

            moto.Placa = license;
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Removes a motorcycle from the repository by its identifier.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <returns>True if the removal was successful, otherwise false.</returns>
        public async Task<bool> RemoveMotoAsync(string identifier)
        {
            var moto = await FindByMotoIdentifierAsync(identifier);
            if (moto != null)
            {
                _motos.Remove(moto);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }
}