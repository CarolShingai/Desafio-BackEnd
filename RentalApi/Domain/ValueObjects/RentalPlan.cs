namespace RentalApi.Domain.ValueObjects
{
	public class RentalPlan
	{
		public int Days { get; set; }
		public decimal DailyRate { get; set; }
		public decimal EarlyReturnPenalty { get; set; }
		public decimal AfterReturnPenalty { get; set; } = 50;

		public static RentalPlan GetPlan(int days)
		{
			return days switch
			{
				7 => new RentalPlan { Days = 7, DailyRate = 30m, EarlyReturnPenalty = 0.2m },
				15 => new RentalPlan { Days = 15, DailyRate = 28m, EarlyReturnPenalty = 0.4m },
				30 => new RentalPlan { Days = 30, DailyRate = 22m, EarlyReturnPenalty = 0m },
				45 => new RentalPlan { Days = 45, DailyRate = 20m, EarlyReturnPenalty = 0m },
				50 => new RentalPlan { Days = 50, DailyRate = 18m, EarlyReturnPenalty = 0m },
				_ => throw new ArgumentException($"Plano de locação inválido. Planos disponíveis: 7, 15, 30, 45, 50 dias")
			};
		}

		public static List<RentalPlan> GetAllPlans()
		{
			return new List<RentalPlan>
			{
				GetPlan(7),
				GetPlan(15),
				GetPlan(30),
				GetPlan(45),
				GetPlan(50)
			};
		}
		public decimal CalculateEarlyReturnPenalty(int unusedDays)
		{
			if (EarlyReturnPenalty > 0 && unusedDays > 0)
				return unusedDays * DailyRate * EarlyReturnPenalty;
			return 0m;
		}
		public decimal CalculateLateReturnPenalty(int extraDays)
		{
			if (extraDays > 0)
				return extraDays * AfterReturnPenalty;
			return 0m;
		}
	}
}
