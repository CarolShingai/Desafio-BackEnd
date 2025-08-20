using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace RentalApi.Application.Services
{
    /// <summary>
    /// Service class for business logic related to Moto operations.
    /// </summary>
    public class MotoService : IMotoService
    {
        public readonly IMotoRepository _motoRepository;
            public IMessagePublisher _messagePublisher;
            
        /// <summary>
        /// Initializes a new instance of the MotoService class.
        /// </summary>
        /// <param name="motoRepository">Repository for Moto operations.</param>
        /// <param name="messagePublisher">Message publisher for publishing domain events.</param>
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
            var currentYear = DateTime.UtcNow.Year;
            if (moto.Year < 2000 || moto.Year > currentYear)
                throw new Exception
                    ("Invalid year. The motorcycle year must be between 2000 and the current year.");

            // Sanitize license plate first
            moto.LicensePlate = moto.LicensePlate.Trim().ToUpperInvariant();
            
            if (!Regex.IsMatch(moto.LicensePlate, @"^[A-Z]{3}-\d{4}$"))
                throw new Exception("Invalid license plate format. Use the format XXX-1111.");

            var motoExist = await _motoRepository.FindByMotoLicenseAsync(moto.LicensePlate);
            if (motoExist != null)
                throw new Exception("The motorcycle with the same license plate already exists.");

            moto.Message = $"Moto com placa {moto.LicensePlate} registrada!";
            moto.NotifiedAt = DateTime.UtcNow;
            await _messagePublisher.PublishAsync(moto, "motoQueue");
            return await _motoRepository.AddMotoAsync(moto);
        }

        /// <summary>
        /// Updates the license plate of an existing motorcycle.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle.</param>
        /// <param name="license">New license plate to assign.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        /// <exception cref="Exception">Thrown if the motorcycle is not found, license format is invalid, or license already exists.</exception>
        public async Task<bool> ChangeMotoLicenseAsync(string identifier, string license)
        {
            license = license.Trim().ToUpperInvariant();

            if (!Regex.IsMatch(license, @"^[A-Z]{3}-\d{4}$"))
                throw new Exception("Invalid license plate format. Use the format XXX-1111.");

            var motoExist = await _motoRepository.FindByMotoIdentifierAsync(identifier);
            if (motoExist == null)
                throw new Exception("Motorcycle not found.");

            var motoSameLicense = await _motoRepository.FindByMotoLicenseAsync(license);
            if (motoSameLicense != null && motoSameLicense.Identifier != identifier)
                throw new Exception("License plate already exists on another motorcycle.");
            return await _motoRepository.UpdateMotoLicenseAsync(identifier, license);
        }

        /// <summary>
        /// Deletes a registered motorcycle from the system.
        /// </summary>
        /// <param name="identifier">Unique identifier of the motorcycle to delete.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
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
