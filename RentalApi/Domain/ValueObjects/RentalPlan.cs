namespace RentalApi.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing a motorcycle rental plan.
    /// Contains pricing information and penalty calculations for different rental durations.
    /// </summary>
    public class RentalPlan
    {
        /// <summary>
        /// Number of days in the rental plan.
        /// </summary>
        public int Days { get; set; }
        
        /// <summary>
        /// Daily rate for the rental plan in Brazilian Reais.
        /// </summary>
        public decimal DailyRate { get; set; }
        
        /// <summary>
        /// Penalty percentage applied for early returns (as a decimal, e.g., 0.2 = 20%).
        /// </summary>
        public decimal EarlyReturnPenalty { get; set; }
        
        /// <summary>
        /// Daily penalty amount for late returns in Brazilian Reais.
        /// </summary>
        public decimal AfterReturnPenalty { get; set; } = 50;

        /// <summary>
        /// Gets a rental plan configuration for the specified number of days.
        /// </summary>
        /// <param name="days">Number of days for the rental plan (7, 15, 30, 45, or 50).</param>
        /// <returns>The corresponding RentalPlan with pricing and penalty information.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid plan duration is specified.</exception>
        public static RentalPlan GetPlan(int days)
        {
            return days switch
            {
                7 => new RentalPlan { Days = 7, DailyRate = 30m, EarlyReturnPenalty = 0.2m },
                15 => new RentalPlan { Days = 15, DailyRate = 28m, EarlyReturnPenalty = 0.4m },
                30 => new RentalPlan { Days = 30, DailyRate = 22m, EarlyReturnPenalty = 0m },
                45 => new RentalPlan { Days = 45, DailyRate = 20m, EarlyReturnPenalty = 0m },
                50 => new RentalPlan { Days = 50, DailyRate = 18m, EarlyReturnPenalty = 0m },
                _ => throw new ArgumentException($"Invalid rental plan. Available plans: 7, 15, 30, 45, 50 days")
            };
        }

        /// <summary>
        /// Gets all available rental plans.
        /// </summary>
        /// <returns>A list of all available rental plans.</returns>
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
        
        /// <summary>
        /// Calculates the penalty amount for early return of a rental.
        /// </summary>
        /// <param name="unusedDays">Number of unused days from the original rental plan.</param>
        /// <returns>The penalty amount in Brazilian Reais.</returns>
        public decimal CalculateEarlyReturnPenalty(int unusedDays)
        {
            if (EarlyReturnPenalty > 0 && unusedDays > 0)
                return unusedDays * DailyRate * EarlyReturnPenalty;
            return 0m;
        }
        
        /// <summary>
        /// Calculates the penalty amount for late return of a rental.
        /// </summary>
        /// <param name="extraDays">Number of days beyond the expected return date.</param>
        /// <returns>The penalty amount in Brazilian Reais.</returns>
        public decimal CalculateLateReturnPenalty(int extraDays)
        {
            if (extraDays > 0)
                return extraDays * AfterReturnPenalty;
            return 0m;
        }
    }
}
