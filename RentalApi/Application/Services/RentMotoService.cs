using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Domain.ValueObjects;

namespace RentalApi.Application.Services
{
	public class RentMotoService : IRentMotoService
	{
		private readonly IRentMotoRepository _rentMotoRepository;
		private readonly IDeliveryPersonRepository _deliveryRepo;

		public RentMotoService(IRentMotoRepository rentMotoRepository, IDeliveryPersonRepository deliveryPersonRepository)
		{
			_rentMotoRepository = rentMotoRepository;
			_deliveryRepo = deliveryPersonRepository;
		}
		public async Task<RentMoto> CreateRentalAsync(string deliveryPersonId, string motoId, int planDays)
		{
			var deliveryPerson = await _deliveryRepo.FindDeliveryPersonByIdentifierAsync(deliveryPersonId);
			if (deliveryPerson == null)
				throw new ArgumentException("Entregador não encontrado");

			if (deliveryPerson.CnhType != "A" && deliveryPerson.CnhType != "A+B")
				throw new InvalidOperationException("Somente entregadores habilitados na categoria A podem efetuar uma locação");

			var plan = GetRentalPlan(planDays);
			var startDate = DateTime.UtcNow.Date.AddDays(1);
			var expectedEndDate = startDate.AddDays(planDays);

			var rental = new RentMoto
			{
				Id = Guid.NewGuid(),
				RentId = Guid.NewGuid().ToString(),
				MotoId = int.Parse(motoId),
				DeliveryPersonId = Guid.Parse(deliveryPersonId),
				StartDate = startDate,
				ExpectedReturnDate = expectedEndDate,
				PlanDays = planDays,
				DailyRate = plan.DailyRate,
				PricePerDay = plan.DailyRate,
				TotalCost = plan.DailyRate * planDays,
				IsDeliveryPersonActive = true
			};
			await _rentMotoRepository.AddRentalAsync(rental);
			return rental;
		}

		public async Task<RentMoto> InformReturnDateAsync(string rentId, DateTime actualReturnDate)
		{
			var rental = await _rentMotoRepository.GetRentalByIdAsync(rentId);
			if (rental == null)
				throw new ArgumentException("Locação não encontrada");

			if (rental.ActualReturnDate.HasValue)
				throw new InvalidOperationException("Data de devolução já foi informada para esta locação");

			rental.ActualReturnDate = actualReturnDate;
			rental.TotalCost = CalculateRentalValue(rental, actualReturnDate);
			await _rentMotoRepository.UpdateRentAsync(rental);
			return rental;
		}

		public async Task<decimal> GetFinalRentalValueAsync(string rentId)
		{
			var rental = await _rentMotoRepository.GetRentalByIdAsync(rentId);
			if (rental == null)
				throw new ArgumentException("Locação não encontrada");

			if (!rental.ActualReturnDate.HasValue)
				throw new InvalidOperationException("Data de devolução ainda não foi informada");

			return rental.TotalCost;
		}

		public async Task<decimal> SimulateReturnValueAsync(string rentId, DateTime returnDate)
		{
			var rental = await _rentMotoRepository.GetRentalByIdAsync(rentId);
			if (rental == null)
				throw new ArgumentException("Locação não encontrada");

			return CalculateRentalValue(rental, returnDate);
		}

		private RentalPlan GetRentalPlan(int days)
		{
			return RentalPlan.GetPlan(days);
		}

		private decimal CalculateRentalValue(RentMoto rental, DateTime returnDate)
		{
			if (!rental.ExpectedReturnDate.HasValue)
				throw new InvalidOperationException("Data prevista de devolução não definida");

			var plan = GetRentalPlan(rental.PlanDays);
			var baseCost = plan.DailyRate * rental.PlanDays;
			if (returnDate.Date < rental.ExpectedReturnDate.Value.Date)
			{
				var unusedDays = (rental.ExpectedReturnDate.Value.Date - returnDate.Date).Days;
				var penalty = plan.CalculateEarlyReturnPenalty(unusedDays);
				return baseCost + penalty;
			}
			else if (returnDate.Date > rental.ExpectedReturnDate.Value.Date)
			{
				var extraDays = (returnDate.Date - rental.ExpectedReturnDate.Value.Date).Days;
				var lateFee = plan.CalculateLateReturnPenalty(extraDays);
				return baseCost + lateFee;
			}
			else
				return baseCost;
		}
	}
}
