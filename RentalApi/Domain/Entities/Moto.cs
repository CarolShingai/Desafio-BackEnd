
namespace RentalApi.Domain.Entities
{
    /// <summary>
    /// Represents a motorcycle entity in the rental system.
    /// </summary>
    public class Moto
    {
        /// <summary>
        /// Primary key identifier for the motorcycle.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique identifier for the motorcycle (business key).
        /// </summary>
        public string Identificador { get; set; } = string.Empty;

        /// <summary>
        /// Manufacturing year of the motorcycle.
        /// </summary>
        public int Ano { get; set; }

        /// <summary>
        /// Model name of the motorcycle.
        /// </summary>
        public string Modelo { get; set; } = string.Empty;

        /// <summary>
        /// License plate of the motorcycle.
        /// </summary>
        public string Placa { get; set; } = string.Empty;

        public DateTime NotifiedAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
