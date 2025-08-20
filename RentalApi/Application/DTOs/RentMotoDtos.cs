using System.Text.Json.Serialization;

namespace RentalApi.Application.DTOs
{
    /// <summary>
    /// DTO for creating a new motorcycle rental request.
    /// </summary>
    public class CreateRentalRequest
    {
        /// <summary>
        /// Unique identifier of the delivery person requesting the rental.
        /// </summary>
        [JsonPropertyName("id_entregador")]
        public string DeliveryPersonId { get; set; } = string.Empty;

        /// <summary>
        /// Unique identifier of the motorcycle to be rented.
        /// </summary>
        [JsonPropertyName("id_moto")]
        public string MotoId { get; set; } = string.Empty;

        /// <summary>
        /// Number of days for the rental plan (7, 15, 30, 45, or 50 days).
        /// </summary>
        [JsonPropertyName("plano")]
        public int PlanDays { get; set; }
    }

    /// <summary>
    /// DTO for returning rental information in API responses.
    /// </summary>
    public class RentalResponse
    {
        /// <summary>
        /// Unique identifier of the rental.
        /// </summary>
        [JsonPropertyName("id_locacao")]
        public string RentId { get; set; } = string.Empty;

        /// <summary>
        /// Unique identifier of the rented motorcycle.
        /// </summary>
        [JsonPropertyName("id_moto")]
        public string MotoId { get; set; } = string.Empty;

        /// <summary>
        /// Unique identifier of the delivery person who rented the motorcycle.
        /// </summary>
        [JsonPropertyName("id_entregador")]
        public string DeliveryPersonId { get; set; } = string.Empty;

        /// <summary>
        /// Start date of the rental period.
        /// </summary>
        [JsonPropertyName("data_inicio")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Expected return date of the motorcycle.
        /// </summary>
        [JsonPropertyName("data_prevista_devolucao")]
        public DateTime ExpectedReturnDate { get; set; }

        /// <summary>
        /// Number of days in the rental plan.
        /// </summary>
        [JsonPropertyName("plano")]
        public int PlanDays { get; set; }

        /// <summary>
        /// Daily rate for the rental plan.
        /// </summary>
        [JsonPropertyName("valor_diaria")]
        public decimal DailyRate { get; set; }

        /// <summary>
        /// Estimated total cost of the rental without penalties or discounts.
        /// </summary>
        [JsonPropertyName("valor_estimado_total")]
        public decimal EstimatedTotalCost { get; set; }
    }

    /// <summary>
    /// DTO for informing the actual return date of a rented motorcycle.
    /// </summary>
    public class InformReturnDateRequest
    {
        /// <summary>
        /// Actual date when the motorcycle was returned.
        /// </summary>
        [JsonPropertyName("data_retorno")]
        public DateTime ActualReturnDate { get; set; }
    }

    /// <summary>
    /// DTO for simulating rental return value calculation.
    /// </summary>
    public class ReturnSimulationRequest
    {
        /// <summary>
        /// Proposed return date for value simulation.
        /// </summary>
        [JsonPropertyName("data_retorno")]
        public DateTime ReturnDate { get; set; }
    }

    /// <summary>
    /// DTO for returning calculated rental value with penalties or discounts.
    /// </summary>
    public class RentalValueResponseDto
    {
        /// <summary>
        /// Total calculated value for the rental including any penalties or discounts.
        /// </summary>
        [JsonPropertyName("valor_total")]
        public decimal TotalValue { get; set; }
        
        /// <summary>
        /// Detailed breakdown of the calculation including base cost, penalties, and discounts.
        /// </summary>
        [JsonPropertyName("detalhes")]
        public string Details { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// DTO for returning error messages in rental API responses.
    /// </summary>
    public class RentalErrorResponse
    {
        /// <summary>
        /// Error message describing what went wrong.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
