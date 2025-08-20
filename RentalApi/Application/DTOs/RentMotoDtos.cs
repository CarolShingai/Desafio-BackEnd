namespace RentalApi.Application.DTOs
{
    public class CreateRentalRequestDto
    {
        public string DeliveryPersonId { get; set; } = string.Empty;
        public string MotoId { get; set; } = string.Empty;
        public int PlanDays { get; set; }
    }

    public class RentalResponseDto
    {
        public string RentId { get; set; } = string.Empty;
        public string MotoId { get; set; } = string.Empty;
        public string DeliveryPersonId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public int PlanDays { get; set; }
        public decimal DailyRate { get; set; }
        public decimal EstimatedTotalCost { get; set; }
    }

    public class InformReturnDateRequestDto
    {
        public DateTime ActualReturnDate { get; set; }
    }

    public class ReturnSimulationRequestDto
    {
        public DateTime ReturnDate { get; set; }
    }

    public class RentalValueResponseDto
    {
        public decimal TotalValue { get; set; }
        public string Details { get; set; } = string.Empty;
    }
}
