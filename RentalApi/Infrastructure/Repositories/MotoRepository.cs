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
        public async Task<List<Moto>> FindMotoAllAsync()
        {
            return await Task.FromResult(_motos);
        }
        public async Task<Moto?> FindByMotoIdentifierAsync(string identifier)
        {
            var moto = _motos.FirstOrDefault(m => m.Identificador == identifier);
            return await Task.FromResult(moto);
        }
        public async Task<Moto?> FindByMotoLicenseAsync(string license)
        {
            var moto = _motos.FirstOrDefault(m => m.Placa == license);
            return await Task.FromResult(moto);
        }
        public async Task<List<Moto>> SearchMotosByLicenseAsync(string license)
        {
            var result = _motos.Where(m => m.Placa.Contains(license)).ToList();
            return await Task.FromResult(result);
        }
        public async Task<Moto> AddMotoAsync(Moto moto)
        {
            moto.Id = _nextId++;
            _motos.Add(moto);
            return await Task.FromResult(moto);
        }
        public async Task<bool> UpdateMotoLicenseAsync(string identifier, string license)
        {
            var moto = await FindByMotoIdentifierAsync(identifier);
            if (moto == null) return false;

            var equalLicense = await SearchMotosByLicenseAsync(license);
            if (equalLicense != null) return false;

            moto.Placa = license;
            return await Task.FromResult(true);
        }

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