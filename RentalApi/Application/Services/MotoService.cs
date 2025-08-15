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
            return await _motoRepository.FindMotoAllAsync();
        }
        public async Task<Moto?> GetMotoByIdentifierAsync(string identifier)
        {
            return await _motoRepository.FindByMotoIdentifierAsync(identifier);
        }
        public async Task<Moto> RegisterNewMotoAsync(Moto moto)
        {
            var motoExist = await _motoRepository.FindByMotoLicenseAsync(moto.Placa);
            if (motoExist != null)
            {
                throw new Exception("The motorcycle with the same license plate already exists.");
            }
            return await _motoRepository.AddMotoAsync(moto);
        }
        public async Task<bool> ChangeMotoLicenseAsync(string identifier, string license)
        {
            var motoExist = await _motoRepository.FindByMotoIdentifierAsync(identifier);
            if (motoExist == null)
            {
                throw new Exception("Motorcycle not found.");
            }
            var motoSameLicense = await _motoRepository.FindByMotoLicenseAsync(license);
            if (motoSameLicense != null && motoSameLicense.Identificador != identifier)
            {
                throw new Exception("License plate already exists on another motorcycle.");
            }
            return await _motoRepository.UpdateMotoLicenseAsync(identifier, license);
        }
        public async Task<bool> DeleteRegisteredMotoAsync(string identifier)
        {
            var motoExist = await _motoRepository.FindByMotoIdentifierAsync(identifier);
            if (motoExist == null)
            {
                throw new Exception("Motorcycle not found.");
            }
            return await _motoRepository.RemoveMotoAsync(identifier);
        }
        
    }
}