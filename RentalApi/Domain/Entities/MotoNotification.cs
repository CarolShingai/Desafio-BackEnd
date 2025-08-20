namespace RentalApi.Domain.Entities
{
    /// <summary>
    /// Represents a motorcycle notification entity used for messaging and event tracking.
    /// Stores information about motorcycle registration events for audit and notification purposes.
    /// </summary>
    public class MotoNotification
    {
        /// <summary>
        /// Primary key identifier for the notification.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Foreign key reference to the motorcycle entity.
        /// </summary>
        public int MotorcycleId { get; set; }
        
        /// <summary>
        /// Unique business identifier of the motorcycle.
        /// </summary>
        public string MotorcycleIdentifier { get; set; } = string.Empty;
        
        /// <summary>
        /// Manufacturing year of the motorcycle.
        /// </summary>
        public int Year { get; set; }
        
        /// <summary>
        /// Model name of the motorcycle.
        /// </summary>
        public string Model { get; set; } = string.Empty;
        
        /// <summary>
        /// License plate of the motorcycle.
        /// </summary>
        public string LicensePlate { get; set; } = string.Empty;
        
        /// <summary>
        /// Timestamp when the notification was created.
        /// </summary>
        public DateTime NotifiedAt { get; set; }
        
        /// <summary>
        /// Notification message describing the event.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Default constructor for entity framework and serialization.
        /// </summary>
        public MotoNotification() {}

        /// <summary>
        /// Constructor for creating a new motorcycle notification with all required information.
        /// </summary>
        /// <param name="motorcycleId">ID of the motorcycle being notified about.</param>
        /// <param name="motorcycleIdentifier">Unique business identifier of the motorcycle.</param>
        /// <param name="year">Manufacturing year of the motorcycle.</param>
        /// <param name="model">Model name of the motorcycle.</param>
        /// <param name="licensePlate">License plate of the motorcycle.</param>
        public MotoNotification(int motorcycleId, string motorcycleIdentifier, int year, string model, string licensePlate)
        {
            Id = Guid.NewGuid();
            MotorcycleId = motorcycleId;
            MotorcycleIdentifier = motorcycleIdentifier;
            Year = year;
            Model = model;
            LicensePlate = licensePlate;
            NotifiedAt = DateTime.UtcNow;
            Message = $"Motorcycle from {year} has been registered: {model} - {licensePlate}";
        }
    }
}
