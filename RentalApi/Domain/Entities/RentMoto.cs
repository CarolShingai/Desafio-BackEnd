namespace RentalApi.Domain.Entities
{
    public class RentMoto
    {
        public Guid Id { get; set; }
        public string RentId { get; set; } = string.Empty;
        public int MotoId { get; set; }
        public Guid DeliveryPersonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }

        public decimal PricePerDay { get; set; }
        public decimal TotalCost { get; set; }
        public int PlanDays { get; set; }
        public decimal DailyRate { get; set; }

        public decimal? FineAmount { get; set; }
        public bool IsDeliveryPersonActive { get; set; } = false;
        public virtual Moto? Moto { get; set; }
        public virtual DeliveryPerson? DeliveryPerson { get; set; }
        public RentMoto() {}


        public RentMoto(string rentId, int motoId, Guid deliveryPersonId, DateTime startDate, decimal pricePerDay)
        {
            Id = Guid.NewGuid();
            RentId = rentId;
            MotoId = motoId;
            DeliveryPersonId = deliveryPersonId;
            StartDate = startDate;
            PricePerDay = pricePerDay;
            IsDeliveryPersonActive = false;
        }
    }
}