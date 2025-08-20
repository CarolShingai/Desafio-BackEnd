using System.Text.Json.Serialization;

namespace RentalApi.Application.DTOs
{
    public class CreateRentalRequest
    {
        [JsonPropertyName("id_entregador")]
        public string DeliveryPersonId { get; set; } = string.Empty;

        [JsonPropertyName("id_moto")]
        public string MotoId { get; set; } = string.Empty;

        [JsonPropertyName("plano")]
        public int PlanDays { get; set; }
    }

    public class RentalResponse
    {
        [JsonPropertyName("id_locacao")]
        public string RentId { get; set; } = string.Empty;

        [JsonPropertyName("id_moto")]
        public string MotoId { get; set; } = string.Empty;

        [JsonPropertyName("id_entregador")]
        public string DeliveryPersonId { get; set; } = string.Empty;

        [JsonPropertyName("data_inicio")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("data_prevista_devolucao")]
        public DateTime ExpectedReturnDate { get; set; }

        [JsonPropertyName("dias_plano")]
        public int PlanDays { get; set; }

        [JsonPropertyName("valor_diaria")]
        public decimal DailyRate { get; set; }

        [JsonPropertyName("valor_estimado_total")]
        public decimal EstimatedTotalCost { get; set; }
    }

    public class InformReturnDateRequest
    {
        public DateTime ActualReturnDate { get; set; }
    }

    public class ReturnSimulationRequest
    {
        public DateTime ReturnDate { get; set; }
    }

    public class RentalValueResponseDto
    {
        public decimal TotalValue { get; set; }
        public string Details { get; set; } = string.Empty;
    }
    public class RentalErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
