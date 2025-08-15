using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IMotoRepository
    {
        Task<List<Moto>> FindMotoAllAsync();
        Task<Moto?> FindByMotoIdAsync(int id);
        Task<Moto?> FindByMotoLicenseAsync(string license);
        Task<Moto> AddMotoAsync(Moto moto);
        Task<bool> UpdateMotoLicenseAsync(int id, string license);
        Task<bool> RemoveMotoAsync(int id);
    }
}