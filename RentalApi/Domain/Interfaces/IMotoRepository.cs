using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IMotoRepository
    {
        Task<List<Moto>> FindMotoAllAsync();
        Task<Moto?> FindByMotoIdentifierAsync(string identifier);
        Task<Moto?> FindByMotoLicenseAsync(string license);
        Task<List<Moto>> SearchMotosByLicenseAsync(string license); //chnage
        Task<Moto> AddMotoAsync(Moto moto);
        Task<bool> UpdateMotoLicenseAsync(string identifier, string license);
        Task<bool> RemoveMotoAsync(string identifier);
    }
}