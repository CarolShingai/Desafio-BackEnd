namespace RentalApi.Domain.Entities
{
    public class RentMoto
    {
        public Guid Id { get; set; }
        public string MotoId { get; set; } = string.Empty;
        public string DeliveryPersonId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }

        public decimal PricePerDay { get; set; }
        public decimal TotalCost { get; set; }

        public decimal? FineAmount { get; set; }
        public bool IsDeliveryPersonActive { get; set; } = false;

        public RentMoto() {}

        public RentMoto(string motoId, string deliveryPersonId, DateTime startDate, decimal pricePerDay)
        {
            Id = Guid.NewGuid();
            MotoId = motoId;
            DeliveryPersonId = deliveryPersonId;
            StartDate = startDate;
            PricePerDay = pricePerDay;
            IsDeliveryPersonActive = false;
        }
    }
}