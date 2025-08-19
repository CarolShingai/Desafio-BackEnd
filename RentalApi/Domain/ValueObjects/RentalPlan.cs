namespace RentalApi.Domain.ValueObjects
{
	public class RentalPlan
	{
		public int Days { get; set; }
		public decimal DailyRate { get; set; }
		public decimal EarlyReturnPenalty { get; set; }
	};

	private readonly List<RentalPlan> _rentalPlans = new()
	{
		new RentalPlan { Days = 7, DailyRate = 30m, EarlyReturnPenalty = 0.2m },
		new RentalPlan { Days = 15, DailyRate = 28m, EarlyReturnPenalty = 0.4m },
		new RentalPlan { Days = 30, DailyRate = 22m, EarlyReturnPenalty = 0m },
		new RentalPlan { Days = 45, DailyRate = 20m, EarlyReturnPenalty = 0m },
		new RentalPlan { Days = 50, DailyRate = 18m, EarlyReturnPenalty = 0m }
	};
}
