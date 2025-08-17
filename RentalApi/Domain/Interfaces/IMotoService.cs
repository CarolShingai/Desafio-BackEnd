using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IMotoService
    {
        Task<List<Moto>> GetAllMoto();
        Task<Moto?> GetMotoByIdentifierAsync(string identifier);
        Task<Moto> RegisterNewMotoAsync(Moto moto);
        Task<bool> ChangeMotoLicenseAsync(string identifier, string license);
        Task<bool> DeleteRegisteredMotoAsync(string identifier);
    }
}