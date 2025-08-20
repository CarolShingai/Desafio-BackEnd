using System.Text.Json.Serialization;

namespace RentalApi.Application.DTOs
{
    public class CreateDeliveryPersonRequest
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [JsonPropertyName("data_nascimento")]
        public DateTime BirthDate { get; set; } = DateTime.MinValue;

        [JsonPropertyName("numero_cnh")]
        public string CnhNumber { get; set; } = string.Empty;

        [JsonPropertyName("tipo_cnh")]
        public string CnhType { get; set; } = string.Empty;

        [JsonPropertyName("imagem_cnh")]
        public string CnhImage { get; set; } = string.Empty;
    }

    public class DeliveryPersonResponse
    {
        public Guid Id { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string Cnh { get; set; } = string.Empty;
        public string CnhType { get; set; } = string.Empty;
        public string CnhImage { get; set; } = string.Empty;
    }

    public class UpdateImageRequest
    {
        public string DeliveryPersonId { get; set; } = string.Empty;
        public string UpdateCnhImageImage { get; set; } = string.Empty;
    }

    public class DeliveryErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class DeliverySuccessResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}