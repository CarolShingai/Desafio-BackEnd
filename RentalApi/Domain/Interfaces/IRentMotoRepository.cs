using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IRentMotoRepository
    {
        Task<RentMoto> AddRentalAsync(RentMoto rentMoto);
        Task<RentMoto?> FindRentalByIdAsync(string id);
        Task<RentMoto> UpdateRentAsync(RentMoto rentMoto);
    }
}
