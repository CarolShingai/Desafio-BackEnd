using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Domain.ValueObjects;

namespace RentalApi.Application.Services
{
    /// <summary>
    /// Service class for business logic related to motorcycle rental operations.
    /// </summary>
    public class RentMotoService : IRentMotoService
    {
        private readonly IRentMotoRepository _rentMotoRepository;
        private readonly IDeliveryPersonRepository _deliveryRepo;
        private readonly IMotoRepository _motoRepository;

        /// <summary>
        /// Initializes a new instance of the RentMotoService class.
        /// </summary>
        /// <param name="rentMotoRepository">Repository for rental operations.</param>
        /// <param name="deliveryPersonRepository">Repository for delivery person operations.</param>
        /// <param name="motoRepository">Repository for motorcycle operations.</param>
        public RentMotoService(IRentMotoRepository rentMotoRepository, IDeliveryPersonRepository deliveryPersonRepository, IMotoRepository motoRepository)
        {
            _rentMotoRepository = rentMotoRepository;
            _deliveryRepo = deliveryPersonRepository;
            _motoRepository = motoRepository;
        }

        /// <summary>
        /// Validates the rental request parameters and entities existence.
        /// </summary>
        /// <param name="deliveryPersonId">Unique identifier of the delivery person.</param>
        /// <param name="motoId">Unique identifier of the motorcycle.</param>
        /// <param name="planDays">Number of days for the rental plan.</param>
        /// <returns>A tuple containing the validated DeliveryPerson and Moto entities.</returns>
        /// <exception cref="ArgumentException">Thrown when entities are not found or plan is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when delivery person doesn't have the required license.</exception>
        private async Task<(DeliveryPerson, Moto)> ValidateRentalRequest(string deliveryPersonId, string motoId, int planDays)
        {
            var allowedPlans = new[] { 7, 15, 30, 45, 50 };
            if (!allowedPlans.Contains(planDays))
                throw new ArgumentException("Invalid rental plan. Allowed values: 7, 15, 30, 45, 50 days.");

            var deliveryPerson = await _deliveryRepo.FindDeliveryPersonByIdentifierAsync(deliveryPersonId);
            if (deliveryPerson == null)
                throw new ArgumentException("Delivery person not found.");

            if (deliveryPerson.CnhType != "A" && deliveryPerson.CnhType != "A+B")
                throw new InvalidOperationException("Only drivers with category A license can rent a motorcycle.");

            var moto = await _motoRepository.FindByMotoIdentifierAsync(motoId);
            if (moto == null)
                throw new ArgumentException("Motorcycle not found.");

            return (deliveryPerson, moto);
        }
        
        /// <summary>
        /// Creates a new motorcycle rental.
        /// </summary>
        /// <param name="deliveryPersonId">Unique identifier of the delivery person.</param>
        /// <param name="motoId">Unique identifier of the motorcycle.</param>
        /// <param name="planDays">Number of days for the rental plan (7, 15, 30, 45, or 50).</param>
        /// <returns>The created RentMoto entity.</returns>
        public async Task<RentMoto> CreateRentalAsync(string deliveryPersonId, string motoId, int planDays)
        {
            var (deliveryPerson, moto) = await ValidateRentalRequest(deliveryPersonId, motoId, planDays);

            var plan = GetRentalPlan(planDays);
            var startDate = DateTime.UtcNow.Date.AddDays(1);
            var expectedEndDate = startDate.AddDays(planDays);

            var rental = new RentMoto
            {
                Id = Guid.NewGuid(),
                RentId = Guid.NewGuid().ToString(),
				MotoId = moto.Id,
				DeliveryPersonId = deliveryPerson.Id,
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

		/// <summary>
		/// Retrieves a rental by its unique identifier.
		/// </summary>
		/// <param name="rentId">Unique identifier of the rental.</param>
		/// <returns>The RentMoto entity if found, otherwise null.</returns>
		public async Task<RentMoto?> GetRentalByIdAsync(string rentId)
		{
			return await _rentMotoRepository.FindRentalByIdAsync(rentId);
		}

		/// <summary>
		/// Gets the final calculated value of a rental after return.
		/// </summary>
		/// <param name="rentId">Unique identifier of the rental.</param>
		/// <returns>The final calculated rental value including any penalties or discounts.</returns>
		/// <exception cref="ArgumentException">Thrown when rental is not found.</exception>
		/// <exception cref="InvalidOperationException">Thrown when return date has not been provided.</exception>
		public async Task<decimal> GetFinalRentalValueAsync(string rentId)
		{
			var rental = await _rentMotoRepository.FindRentalByIdAsync(rentId);

			if (rental == null)
				throw new ArgumentException("Rental not found");
			if (!rental.ActualReturnDate.HasValue)
				throw new InvalidOperationException("Return date has not been provided yet");
			return rental.TotalCost;
		}
		
		/// <summary>
		/// Informs the actual return date of a rental and calculates the final cost.
		/// </summary>
		/// <param name="rentId">Unique identifier of the rental.</param>
		/// <param name="actualReturnDate">Actual date when the motorcycle was returned.</param>
		/// <returns>The updated RentMoto entity with calculated final cost.</returns>
		/// <exception cref="ArgumentException">Thrown when rental is not found.</exception>
		/// <exception cref="InvalidOperationException">Thrown when return date has already been provided.</exception>
		public async Task<RentMoto> InformReturnDateAsync(string rentId, DateTime actualReturnDate)
		{
			var rental = await _rentMotoRepository.FindRentalByIdAsync(rentId);
			if (rental == null)
				throw new ArgumentException("Rental not found");
			if (rental.ActualReturnDate.HasValue)
				throw new InvalidOperationException("Return date has already been provided for this rental");

			rental.ActualReturnDate = actualReturnDate;
			rental.TotalCost = CalculateRentalValue(rental, actualReturnDate);
			await _rentMotoRepository.UpdateRentAsync(rental);
			return rental;
		}

		/// <summary>
		/// Simulates the rental value calculation for a given return date without updating the rental.
		/// </summary>
		/// <param name="rentId">Unique identifier of the rental.</param>
		/// <param name="returnDate">Proposed return date for simulation.</param>
		/// <returns>The calculated rental value for the proposed return date.</returns>
		/// <exception cref="ArgumentException">Thrown when rental is not found or return date is invalid.</exception>
		public async Task<decimal> SimulateReturnValueAsync(string rentId, DateTime returnDate)
		{
			var rental = await _rentMotoRepository.FindRentalByIdAsync(rentId);
			if (rental == null)
				throw new ArgumentException("Rental not found");
			var sanitizedReturnDate = returnDate.Date;
			if (sanitizedReturnDate < rental.StartDate.Date)
				throw new ArgumentException("Return date cannot be before rental start date");
			// Note: Return date can be after start date - that's normal for rentals
			return CalculateRentalValue(rental, returnDate);
		}

		/// <summary>
		/// Gets the rental plan configuration for a given number of days.
		/// </summary>
		/// <param name="days">Number of days for the rental plan.</param>
		/// <returns>The RentalPlan configuration with daily rates and penalty calculations.</returns>
		private RentalPlan GetRentalPlan(int days)
		{
			return RentalPlan.GetPlan(days);
		}

		/// <summary>
		/// Calculates the total rental value including penalties or discounts based on return date.
		/// </summary>
		/// <param name="rental">The rental entity to calculate for.</param>
		/// <param name="returnDate">The return date to use for calculation.</param>
		/// <returns>The calculated total rental value.</returns>
		/// <exception cref="InvalidOperationException">Thrown when expected return date is not set.</exception>
		private decimal CalculateRentalValue(RentMoto rental, DateTime returnDate)
		{
			if (!rental.ExpectedReturnDate.HasValue)
				throw new InvalidOperationException("Expected return date has not been set");

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
