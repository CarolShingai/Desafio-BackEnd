namespace RentalApi.Domain.Entities
{
    /// <summary>
    /// Represents a delivery person entity in the rental system.
    /// A delivery person can rent motorcycles for work purposes.
    /// </summary>
    public class DeliveryPerson
    {
        /// <summary>
        /// Primary key identifier for the delivery person.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Unique business identifier for the delivery person.
        /// </summary>
        public string Identifier { get; set; } = string.Empty;
        
        /// <summary>
        /// Full name of the delivery person.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// CNPJ (Brazilian company registration number) of the delivery person.
        /// Must be unique in the system.
        /// </summary>
        public string Cnpj { get; set; } = string.Empty;
        
        /// <summary>
        /// Birth date of the delivery person.
        /// Used to verify age requirements for motorcycle rental.
        /// </summary>
        public DateTime BirthDate { get; set; } = DateTime.MinValue;
        
        /// <summary>
        /// Driver's license number (CNH) of the delivery person.
        /// Must be unique in the system.
        /// </summary>
        public string Cnh { get; set; } = string.Empty;
        
        /// <summary>
        /// Type of driver's license (CNH).
        /// Must be 'A' or 'A+B' to qualify for motorcycle rental.
        /// </summary>
        public string CnhType { get; set; } = string.Empty;
        
        /// <summary>
        /// Path or URL to the driver's license image file.
        /// Required for verification purposes.
        /// </summary>
        public string CnhImage { get; set; } = string.Empty;
    }
}