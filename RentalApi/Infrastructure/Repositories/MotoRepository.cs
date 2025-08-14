using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Infrastructure.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly List<Moto> _motos = new List<Moto>();
        private int _nextId = 1;

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
        public async Task<List<Moto>> GetAllAsync()
        {
            return await Task.FromResult(_motos);
        }
        public async Task<Moto?> GetByIdAsync(int id)
        {
            var moto = _motos.FirstOrDefault(m => m.Id == id);
            return await Task.FromResult(moto);
        }
        public async Task<Moto?> GetByLicenseAsync(string placa)
        {
            var moto = _motos.FirstOrDefault(m => m.Placa == placa);
            return await Task.FromResult(moto);
        }
        public async Task<Moto> AddMotoAsync(Moto moto)
        {
            moto.Id = _nextId++;
            _motos.Add(moto);
            return await Task.FromResult(moto);
        }
        public async Task<bool> UpdateMotoLicenseAsync(int id, string license)
        {
            var moto = await GetByIdAsync(id);
            if (moto == null)
            {
                return false;
            }
            var equalLicense = await GetByLicenseAsync(license);
            if (equalLicense != null)
            {
                return false;
            }
            moto.Placa = license;
            return await Task.FromResult(true);
        }

        public async Task<bool> RemoveMotoAsync(int id)
        {
            var moto = await GetByIdAsync(id);
            if (moto != null)
            {
                _motos.Remove(moto);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }
}