namespace RentalApi.Domain.Entities
{
    /// <summary>
    /// Represents a motorcycle rental entity in the system.
    /// Contains all information about a rental contract between a delivery person and a motorcycle.
    /// </summary>
    public class RentMoto
    {
        /// <summary>
        /// Primary key identifier for the rental.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Unique business identifier for the rental.
        /// </summary>
        public string RentId { get; set; } = string.Empty;
        
        /// <summary>
        /// Foreign key to the rented motorcycle.
        /// </summary>
        public int MotoId { get; set; }
        
        /// <summary>
        /// Foreign key to the delivery person who rented the motorcycle.
        /// </summary>
        public Guid DeliveryPersonId { get; set; }
        
        /// <summary>
        /// Start date of the rental period.
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// Expected return date of the motorcycle based on the rental plan.
        /// </summary>
        public DateTime? ExpectedReturnDate { get; set; }
        
        /// <summary>
        /// Actual date when the motorcycle was returned.
        /// Null if not yet returned.
        /// </summary>
        public DateTime? ActualReturnDate { get; set; }

        /// <summary>
        /// Daily price for the rental based on the selected plan.
        /// </summary>
        public decimal PricePerDay { get; set; }
        
        /// <summary>
        /// Total calculated cost of the rental including any penalties or discounts.
        /// </summary>
        public decimal TotalCost { get; set; }
        
        /// <summary>
        /// Number of days in the rental plan (7, 15, 30, 45, or 50).
        /// </summary>
        public int PlanDays { get; set; }
        
        /// <summary>
        /// Daily rate for the rental plan.
        /// </summary>
        public decimal DailyRate { get; set; }

        /// <summary>
        /// Amount of any fines applied to the rental.
        /// </summary>
        public decimal? FineAmount { get; set; }
        
        /// <summary>
        /// Indicates whether the delivery person is active in the system.
        /// </summary>
        public bool IsDeliveryPersonActive { get; set; } = false;
        
        /// <summary>
        /// Navigation property to the rented motorcycle.
        /// </summary>
        public virtual Moto? Moto { get; set; }
        
        /// <summary>
        /// Navigation property to the delivery person who rented the motorcycle.
        /// </summary>
        public virtual DeliveryPerson? DeliveryPerson { get; set; }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public RentMoto() {}

        /// <summary>
        /// Constructor for creating a new rental with basic information.
        /// </summary>
        /// <param name="rentId">Unique business identifier for the rental.</param>
        /// <param name="motoId">ID of the motorcycle to rent.</param>
        /// <param name="deliveryPersonId">ID of the delivery person renting the motorcycle.</param>
        /// <param name="startDate">Start date of the rental.</param>
        /// <param name="pricePerDay">Daily price for the rental.</param>
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