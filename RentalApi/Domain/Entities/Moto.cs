namespace RentalApi.Domain.Entities
{
    /// <summary>
    /// Represents a motorcycle entity in the rental system.
    /// Contains all information about a motorcycle including rental status and messaging properties.
    /// </summary>
    public class Moto
    {
        /// <summary>
        /// Primary key identifier for the motorcycle.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Unique business identifier for the motorcycle.
        /// Used for external identification and API operations.
        /// </summary>
        public string Identifier { get; set; } = string.Empty;
        
        /// <summary>
        /// Manufacturing year of the motorcycle.
        /// Must be between 2000 and current year for rental eligibility.
        /// </summary>
        public int Year { get; set; }
        
        /// <summary>
        /// Model name of the motorcycle (e.g., "Honda CG 160").
        /// </summary>
        public string MotorcycleModel { get; set; } = string.Empty;
        
        /// <summary>
        /// License plate of the motorcycle in Brazilian format (XXX-9999).
        /// Must be unique in the system.
        /// </summary>
        public string LicensePlate { get; set; } = string.Empty;
        
        /// <summary>
        /// Indicates whether the motorcycle is currently rented.
        /// Used to track availability status.
        /// </summary>
        public bool IsRented { get; set; } = false;

        /// <summary>
        /// Timestamp when the motorcycle registration notification was sent.
        /// Used for messaging and audit purposes.
        /// </summary>
        public DateTime NotifiedAt { get; set; }
        
        /// <summary>
        /// Notification message sent when the motorcycle was registered.
        /// Used for messaging and event tracking.
        /// </summary>
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Navigation property to the rental history of this motorcycle.
        /// Contains all past and current rentals associated with this motorcycle.
        /// </summary>
        public virtual ICollection<RentMoto> Rentals { get; set; } = new List<RentMoto>();
    }
}
