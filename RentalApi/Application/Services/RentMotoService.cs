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
				throw new InvalidOperationException("Entregador deve ter CNH tipo A ou A+B para alugar uma moto");

			var plan = GetRentalPlan(planDays);
			var startDate = DateTime.UtcNow;
			var expectedEndDate = startDate.AddDays(planDays);

			var rental = new RentMoto(
				deliveryPersonId,
				int.Parse(motoId),
				Guid.Parse(deliveryPersonId),
				startDate,
				plan.DailyRate
			);

			await _rentMotoRepository.AddRentalAsync(rental);
			return rental;
		}

		public async Task<RentMoto> InformReturnDateAsync(string rentId, DateTime actualReturnDate)
		{
			var rental = await _rentMotoRepository.GetRentalByIdAsync(rentId);
			if (rental == null)
				throw new ArgumentException("Locação não encontrada");

			rental.ActualReturnDate = actualReturnDate;
			await _rentMotoRepository.UpdateRentAsync(rental);
			return rental;
		}

		public async Task<decimal> GetFinalRentalValueAsync(string rentId)
		{
			var rental = await _rentMotoRepository.GetRentalByIdAsync(rentId);
			if (rental == null)
				throw new ArgumentException("Locação não encontrada");

			return CalculateFinalValue(rental);
		}

		public async Task<decimal> SimulateReturnValueAsync(string rentId, DateTime returnDate)
		{
			var rental = await _rentMotoRepository.GetRentalByIdAsync(rentId);
			if (rental == null)
				throw new ArgumentException("Locação não encontrada");

			return SimulateReturnValue(rental, returnDate);
		}

		private RentalPlan GetRentalPlan(int days)
		{
			return days switch
			{
				7 => new RentalPlan { Days = 7, DailyRate = 30m, EarlyReturnPenalty = 0.2m },
				15 => new RentalPlan { Days = 15, DailyRate = 28m, EarlyReturnPenalty = 0.4m },
				30 => new RentalPlan { Days = 30, DailyRate = 22m, EarlyReturnPenalty = 0m },
				45 => new RentalPlan { Days = 45, DailyRate = 20m, EarlyReturnPenalty = 0m },
				50 => new RentalPlan { Days = 50, DailyRate = 18m, EarlyReturnPenalty = 0m },
				_ => throw new ArgumentException("Plano de locação inválido")
			};
		}

		private decimal CalculateFinalValue(RentMoto rental)
		{
			if (rental.ActualReturnDate == null)
				return rental.TotalCost;

			var plan = GetRentalPlan(rental.PlanDays);
			var baseCost = plan.DailyRate * rental.PlanDays;
			
			return baseCost; // Simplificado para este exemplo
		}

		private decimal SimulateReturnValue(RentMoto rental, DateTime returnDate)
		{
			var plan = GetRentalPlan(rental.PlanDays);
			var baseCost = plan.DailyRate * rental.PlanDays;
			
			return baseCost; // Simplificado para este exemplo
		}
	}
}
