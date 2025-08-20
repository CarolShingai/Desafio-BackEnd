
namespace RentalApi.Domain.Events
{
    /// <summary>
    /// Domain event that represents a motorcycle registration event.
    /// This event is published when a new motorcycle is successfully registered in the system.
    /// </summary>
    public class MotoRegisteredEvent
    {
        /// <summary>
        /// Unique identifier of the registered motorcycle.
        /// </summary>
        public string Id { get; set; } = string.Empty;
        
        /// <summary>
        /// Manufacturing year of the registered motorcycle.
        /// </summary>
        public int Year { get; set; }
        
        /// <summary>
        /// Model name of the registered motorcycle.
        /// </summary>
        public string Model { get; set; } = string.Empty;
        
        /// <summary>
        /// License plate of the registered motorcycle.
        /// </summary>
        public string LicensePlate { get; set; } = string.Empty;
        
        /// <summary>
        /// Timestamp when the motorcycle was registered and this event was created.
        /// </summary>
        public DateTime NotifiedAt { get; set; }

        /// <summary>
        /// Default constructor for serialization and deserialization.
        /// </summary>
        public MotoRegisteredEvent() {}

        /// <summary>
        /// Constructor for creating a new motorcycle registered event.
        /// </summary>
        /// <param name="id">Unique identifier of the registered motorcycle.</param>
        /// <param name="year">Manufacturing year of the motorcycle.</param>
        /// <param name="model">Model name of the motorcycle.</param>
        /// <param name="licensePlate">License plate of the motorcycle.</param>
        public MotoRegisteredEvent(string id, int year, string model, string licensePlate)
        {
            Id = id;
            Year = year;
            Model = model;
            LicensePlate = licensePlate;
            NotifiedAt = DateTime.UtcNow;
        }
    }
}
