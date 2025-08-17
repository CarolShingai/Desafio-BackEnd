using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Application.Services
{
    /// <summary>
    /// Service class for business logic related to Moto operations.
    /// </summary>
    public class MotoService
    {
        /// <summary>
        /// Repository for Moto data access.
        /// </summary>
        public readonly IMotoRepository _motoRepository;
            public IMessagePublisher _messagePublisher;
        /// <summary>
        /// Initializes a new instance of the MotoService class.
        /// </summary>
        /// <param name="motoRepository">Repository for Moto operations.</param>
        public MotoService(IMotoRepository motoRepository, IMessagePublisher messagePublisher)
        {
            _motoRepository = motoRepository;
            _messagePublisher = messagePublisher;
        }
        /// <summary>
        /// Gets all motorcycles from the repository.
        /// </summary>
        /// <returns>List of all Moto entities.</returns>
        public async Task<List<Moto>> GetAllMoto()
        {
            return await _motoRepository.FindMotoAllAsync();
        }

        /// <summary>
        /// Gets a motorcycle by its unique identifier.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <returns>The Moto entity if found, otherwise null.</returns>
        public async Task<Moto?> GetMotoByIdentifierAsync(string identifier)
        {
            return await _motoRepository.FindByMotoIdentifierAsync(identifier);
        }

        /// <summary>
        /// Registers a new motorcycle, ensuring the license plate is unique.
        /// </summary>
        /// <param name="moto">Moto entity to register.</param>
        /// <returns>The registered Moto entity.</returns>
        /// <exception cref="Exception">Thrown if a motorcycle with the same license plate already exists.</exception>
        public async Task<Moto> RegisterNewMotoAsync(Moto moto)
        {
            var motoExist = await _motoRepository.FindByMotoLicenseAsync(moto.Placa);
            if (motoExist != null)
                throw new Exception("The motorcycle with the same license plate already exists.");
            if (moto.Ano == 2024)
            {
                // Publish a message to the queue
                var message = new { Texto = "Moto registrada!", Dados = moto };
                await _messagePublisher.PublishAsync(message, "moto_queue");
            }
            return await _motoRepository.AddMotoAsync(moto);
        }

        /// <summary>
        /// Changes the license plate of a motorcycle, ensuring uniqueness.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <param name="license">New license plate.</param>
        /// <returns>True if the update was successful.</returns>
        /// <exception cref="Exception">Thrown if the motorcycle is not found or the license plate already exists on another motorcycle.</exception>
        public async Task<bool> ChangeMotoLicenseAsync(string identifier, string license)
        {
            var motoExist = await _motoRepository.FindByMotoIdentifierAsync(identifier);
            if (motoExist == null)
                throw new Exception("Motorcycle not found.");
            var motoSameLicense = await _motoRepository.FindByMotoLicenseAsync(license);
            if (motoSameLicense != null && motoSameLicense.Identificador != identifier)
                throw new Exception("License plate already exists on another motorcycle.");
            return await _motoRepository.UpdateMotoLicenseAsync(identifier, license);
        }

        /// <summary>
        /// Deletes a registered motorcycle by its identifier.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <returns>True if the deletion was successful.</returns>
        /// <exception cref="Exception">Thrown if the motorcycle is not found.</exception>
        public async Task<bool> DeleteRegisteredMotoAsync(string identifier)
        {
            var motoExist = await _motoRepository.FindByMotoIdentifierAsync(identifier);
            if (motoExist == null)
                throw new Exception("Motorcycle not found.");
            return await _motoRepository.RemoveMotoAsync(identifier);
        }
    }
}
