using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IRentMotoService
    {
        Task<RentMoto> CreateRentalAsync(string deliveryPersonId, string motoId, int planDays);
        Task<RentMoto> InformReturnDateAsync(string rentId, DateTime actualReturnDate);
        Task<decimal> GetFinalRentalValueAsync(string rentId);
        Task<decimal> SimulateReturnValueAsync(string rentId, DateTime ReturnDate);
    }
}
