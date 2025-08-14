using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Application.Services
{
    public class MotoService
    {
        public readonly IMotoRepository _motoRepository;

        public MotoService(IMotoRepository motoRepository)
        {
            _motoRepository = motoRepository;
        }
        public async Task<List<Moto>> GetAllMoto()
        {
            return await _motoRepository.GetAllAsync();
        }
        public async Task<Moto?> GetMotoByIdAsync(int id)
        {
            return await _motoRepository.GetByIdAsync(id);
        }
        public async Task<Moto> RegisterNewMotoAsync(Moto moto)
        {
            var motoExist = await _motoRepository.GetByLicenseAsync(moto.Placa);
            if (motoExist != null)
            {
                throw new Exception("The motorcycle with the same license plate already exists.");
            }
            return await _motoRepository.AddMotoAsync(moto);
        }
        public async Task<bool> RemoveRegisteredMotoAsync(int id)
        {
            var motoExist = await _motoRepository.GetByIdAsync(id);
            if (motoExist == null)
            {
                throw new Exception("Motorcycle not found.");
            }
            return await _motoRepository.RemoveMotoAsync(id);
        }
    }
}