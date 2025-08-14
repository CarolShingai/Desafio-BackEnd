using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IMotoRepository
    {
        Task<List<Moto>> GetAllAsync(); //mudar depois, colocar um enum
        Task<Moto?> GetByIdAsync(int id);
        Task<Moto?> GetByLicenseAsync(string license);
        Task<Moto> AddMotoAsync(Moto moto);
        Task<bool> UpdateMotoLicenseAsync(int id, string license);
        Task<bool> RemoveMotoAsync(int id);
    }
}