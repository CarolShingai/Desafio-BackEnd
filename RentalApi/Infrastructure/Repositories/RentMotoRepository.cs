using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;

namespace RentalApi.Infrastructure.Repositories
{
    public class RentMotoRepository : IRentMotoRepository
    {
        private readonly RentalDbContext _context;

        public RentMotoRepository(RentalDbContext context)
        {
            _context = context;
        }

        public async Task<RentMoto> AddRentalAsync(RentMoto rentMoto)
        {
            _context.RentMotos.Add(rentMoto);
            await _context.SaveChangesAsync();
            return rentMoto;
        }

        public async Task<RentMoto?> FindRentalByIdAsync(string id)
        {
            return await _context.RentMotos
            .Include(r => r.Moto)
            .Include(r => r.DeliveryPerson)
            .FirstOrDefaultAsync(r => r.RentId == id);
        }

        public async Task<RentMoto> UpdateRentAsync(RentMoto rentMoto)
        {
            _context.RentMotos.Update(rentMoto);
            await _context.SaveChangesAsync();
            return rentMoto;
        }
    }
}