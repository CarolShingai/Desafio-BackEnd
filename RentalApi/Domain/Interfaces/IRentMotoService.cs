using RentalApi.Domain.Entities;

namespace RentalApi.Domain.Interfaces
{
    public interface IRentMotoService
    {
        Task<RentMoto> CreateRentalAsync(string deliveryPersonId, string motoId, int planDays, DateTime expectedEndDate);
        Task<decimal> GetRentalTotalValueAsync(string rentId);
        Task<RentMoto> InformReturnDateAsync(string rentId, DateTime actualReturnDate);
        Task<decimal> CalculatePenaltyOrAdditionalAsync(string rentId, DateTime actualReturnDate);
    }
}
