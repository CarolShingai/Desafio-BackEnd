using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Infrastructure.Data;

namespace RentalApi.Infrastructure.Repositories
{
    public class MotoRepositoryEF : IMotoRepository
    {
        private readonly RentalDbContext _context;

        public MotoRepositoryEF(RentalDbContext context)
        {
            _context = context;
        }
        public async Task<List<Moto>> FindMotoAllAsync()
        {
            return await _context.Motos.ToListAsync();
        }
        public async Task<Moto?> FindByMotoIdentifierAsync(string identifier)
        {
            return await _context.Motos.FirstOrDefaultAsync(m => m.Identificador == identifier);
        }
        public async Task<Moto?> FindByMotoLicenseAsync(string license)
        {
            return await _context.Motos.FirstOrDefaultAsync(m => m.Placa == license);
        }
        public async Task<List<Moto>> SearchMotosByLicenseAsync(string license)
        {
            return await _context.Motos
                .Where(m => m.Placa.Contains(license)).ToListAsync();
        }
        public async Task<Moto> AddMotoAsync(Moto moto)
        {
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
            return moto;
        }
        public async Task<bool> UpdateMotoLicenseAsync(string identifier, string license)
        {
            var moto = await FindByMotoIdentifierAsync(identifier);
            if (moto == null) return false;

            moto.Placa = license;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveMotoAsync(string identifier)
        {
            var moto = await FindByMotoIdentifierAsync(identifier);
            if (moto == null) return false;

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}