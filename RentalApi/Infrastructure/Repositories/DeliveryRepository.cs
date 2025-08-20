using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;

namespace RentalApi.Infrastructure.Repositories
{
    public class DeliveryRepository : IDeliveryPersonRepository
    {
        private readonly RentalDbContext _context;
        public DeliveryRepository(RentalDbContext context)
        {
            _context = context;
        }
        public async Task<DeliveryPerson?> FindDeliveryPersonByIdentifierAsync(string id)
        {
            return await _context.DeliveryPersons.FindAsync(id);
        }
        public async Task<DeliveryPerson?> FindDeliveryPersonByCnpjAsync(string cnpj)
        {
            return await _context.DeliveryPersons.FirstOrDefaultAsync(d => d.Cnpj == cnpj);
        }
        public async Task<DeliveryPerson?> FindDeliveryPersonByCnhAsync(string cnh)
        {
            return await _context.DeliveryPersons.FirstOrDefaultAsync(d => d.Cnh == cnh);
        }
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
        public bool IsValidCnhType(string cnhType)
        {
            var ValidCnhType = new[] { "A", "B", "A + B" };
            return ValidCnhType.Contains(cnhType);
        }
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
        public async Task<bool> AddCnhImageAsync(string deliveryPersonId, string base64Image)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
                throw new ArgumentException("Base64 image cannot be null or empty", nameof(base64Image));

            var deliveryPerson = await FindDeliveryPersonByIdentifierAsync(deliveryPersonId);
            if (deliveryPerson == null)
                return false;

            deliveryPerson.CnhImage = base64Image;
            _context.DeliveryPersons.Update(deliveryPerson);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}