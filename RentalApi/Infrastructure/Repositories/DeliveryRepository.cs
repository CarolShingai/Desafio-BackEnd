using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;

namespace RentalApi.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing delivery person operations in the database.
    /// Provides data access methods for creating, retrieving, validating, and updating delivery person records.
    /// </summary>
    public class DeliveryRepository : IDeliveryPersonRepository
    {
        private readonly RentalDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for delivery person operations.</param>
        public DeliveryRepository(RentalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a delivery person by their unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the delivery person.</param>
        /// <returns>
        /// A task representing the asynchronous operation with the delivery person entity if found,
        /// or null if no delivery person with the specified identifier exists.
        /// </returns>
        public async Task<DeliveryPerson?> FindDeliveryPersonByIdentifierAsync(string id)
        {
            return await _context.DeliveryPersons
                .FirstOrDefaultAsync(d => d.Identifier == id);
        }

        /// <summary>
        /// Retrieves a delivery person by their CNPJ (Brazilian business tax ID) asynchronously.
        /// </summary>
        /// <param name="cnpj">The CNPJ to search for.</param>
        /// <returns>
        /// A task representing the asynchronous operation with the delivery person entity if found,
        /// or null if no delivery person with the specified CNPJ exists.
        /// </returns>
        public async Task<DeliveryPerson?> FindDeliveryPersonByCnpjAsync(string cnpj)
        {
            return await _context.DeliveryPersons.FirstOrDefaultAsync(d => d.Cnpj == cnpj);
        }

        /// <summary>
        /// Retrieves a delivery person by their CNH (Brazilian driver's license) asynchronously.
        /// </summary>
        /// <param name="cnh">The CNH number to search for.</param>
        /// <returns>
        /// A task representing the asynchronous operation with the delivery person entity if found,
        /// or null if no delivery person with the specified CNH exists.
        /// </returns>
        public async Task<DeliveryPerson?> FindDeliveryPersonByCnhAsync(string cnh)
        {
            return await _context.DeliveryPersons.FirstOrDefaultAsync(d => d.Cnh == cnh);
        }

        /// <summary>
        /// Validates if a CNPJ is unique and follows the correct format asynchronously.
        /// </summary>
        /// <param name="cnpj">The CNPJ to validate.</param>
        /// <returns>
        /// A task representing the asynchronous operation with true if the CNPJ is valid and unique,
        /// false otherwise.
        /// </returns>
        public async Task<bool> ValidateCnpjAsync(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;
            var delivery = await FindDeliveryPersonByCnpjAsync(cnpj);
            if (delivery != null)
                return false;
            var cnpjWithoutDots = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpjWithoutDots.Length != 14)
                return false;
            if (!long.TryParse(cnpjWithoutDots, out _))
                return false;
            return true;
        }

        /// <summary>
        /// Validates if a CNH is unique and follows the correct format asynchronously.
        /// </summary>
        /// <param name="cnh">The CNH number to validate.</param>
        /// <returns>
        /// A task representing the asynchronous operation with true if the CNH is valid and unique,
        /// false otherwise.
        /// </returns>
        public async Task<bool> ValidateCnhAsync(string cnh)
        {
            if (string.IsNullOrWhiteSpace(cnh))
                return false;
            var delivery = await FindDeliveryPersonByCnhAsync(cnh);
            if (delivery != null)
                return false;
            var cnhWithoutDots = cnh.Replace(".", "").Replace("-", "");
            if (cnhWithoutDots.Length != 11)
                return false;
            if (!long.TryParse(cnhWithoutDots, out _))
                return false;
            return true;
        }

        /// <summary>
        /// Validates if the CNH type is within the accepted categories for motorcycle operation.
        /// </summary>
        /// <param name="cnhType">The CNH type to validate (A, B, or A + B).</param>
        /// <returns>True if the CNH type is valid for motorcycle operation, false otherwise.</returns>
        public bool IsValidCnhType(string cnhType)
        {
            var ValidCnhType = new[] { "A", "B", "A + B" };
            return ValidCnhType.Contains(cnhType);
        }

        /// <summary>
        /// Adds a new delivery person to the database after validating all required fields asynchronously.
        /// </summary>
        /// <param name="deliveryPerson">The delivery person entity to add.</param>
        /// <returns>A task representing the asynchronous operation with the added delivery person entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when deliveryPerson is null.</exception>
        /// <exception cref="ArgumentException">Thrown when CNPJ, CNH, or CNH type validation fails.</exception>
        public async Task<DeliveryPerson> AddDeliveryPersonAsync(DeliveryPerson deliveryPerson)
        {
            if (deliveryPerson == null)
                throw new ArgumentNullException(nameof(deliveryPerson));
            if (!await ValidateCnpjAsync(deliveryPerson.Cnpj))
                throw new ArgumentException("Invalid CNPJ");
            if (!await ValidateCnhAsync(deliveryPerson.Cnh))
                throw new ArgumentException("Invalid CNH");
            if (!IsValidCnhType(deliveryPerson.CnhType))
                throw new ArgumentException("Invalid CNH Type");

            _context.DeliveryPersons.Add(deliveryPerson);
            await _context.SaveChangesAsync();
            return deliveryPerson;
        }

        /// <summary>
        /// Adds or updates the CNH image for a delivery person asynchronously.
        /// </summary>
        /// <param name="deliveryPersonIdentifier">The unique identifier of the delivery person.</param>
        /// <param name="base64Image">The CNH image encoded in base64 format.</param>
        /// <returns>
        /// A task representing the asynchronous operation with true if the image was successfully added/updated,
        /// false if the delivery person was not found.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when base64Image is null or empty.</exception>
        public async Task<bool> AddCnhImageAsync(string deliveryPersonIdentifier, string base64Image)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
                throw new ArgumentException("Base64 image cannot be null or empty", nameof(base64Image));
            var deliveryPerson = await _context.DeliveryPersons
                .FirstOrDefaultAsync(d => d.Identifier == deliveryPersonIdentifier);
            if (deliveryPerson == null)
                return false;

            deliveryPerson.CnhImage = base64Image;
            _context.DeliveryPersons.Update(deliveryPerson);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}