using System.Text.Json.Serialization;

namespace RentalApi.Application.DTOs
{
    /// <summary>
    /// DTO for creating a new delivery person registration request.
    /// </summary>
    public class CreateDeliveryPersonRequest
    {
        /// <summary>
        /// Unique identifier for the delivery person.
        /// </summary>
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the delivery person.
        /// </summary>
        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ (Brazilian company registration number) of the delivery person.
        /// </summary>
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        /// <summary>
        /// Birth date of the delivery person.
        /// </summary>
        [JsonPropertyName("data_nascimento")]
        public DateTime BirthDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Driver's license number (CNH) of the delivery person.
        /// </summary>
        [JsonPropertyName("numero_cnh")]
        public string CnhNumber { get; set; } = string.Empty;

        /// <summary>
        /// Type of driver's license (CNH). Must be A or A+B for motorcycle rental.
        /// </summary>
        [JsonPropertyName("tipo_cnh")]
        public string CnhType { get; set; } = string.Empty;

        /// <summary>
        /// Base64 encoded image of the driver's license.
        /// </summary>
        [JsonPropertyName("imagem_cnh")]
        public string CnhImage { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for returning delivery person data in API responses.
    /// </summary>
    public class DeliveryPersonResponse
    {
        /// <summary>
        /// Internal system ID of the delivery person.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Unique identifier for the delivery person.
        /// </summary>
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;
        
        /// <summary>
        /// Full name of the delivery person.
        /// </summary>
        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// CNPJ (Brazilian company registration number) of the delivery person.
        /// </summary>
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;
        
        /// <summary>
        /// Birth date of the delivery person as formatted string.
        /// </summary>
        [JsonPropertyName("data_nascimento")]
        public string BirthDate { get; set; } = string.Empty;
        
        /// <summary>
        /// Driver's license number (CNH) of the delivery person.
        /// </summary>
        [JsonPropertyName("numero_cnh")]
        public string Cnh { get; set; } = string.Empty;
        
        /// <summary>
        /// Type of driver's license (CNH). Must be A or A+B for motorcycle rental.
        /// </summary>
        [JsonPropertyName("tipo_cnh")]
        public string CnhType { get; set; } = string.Empty;
        
        /// <summary>
        /// Path or URL to the driver's license image.
        /// </summary>
        [JsonPropertyName("imagem_cnh")]
        public string CnhImage { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating the driver's license image of a delivery person.
    /// </summary>
    public class UpdateImageRequest
    {
        /// <summary>
        /// Unique identifier of the delivery person whose image will be updated.
        /// </summary>
        public string DeliveryPersonId { get; set; } = string.Empty;
        
        /// <summary>
        /// Base64 encoded new driver's license image.
        /// </summary>
        public string UpdateCnhImageImage { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for returning error messages in delivery person API responses.
    /// </summary>
    public class DeliveryErrorResponse
    {
        /// <summary>
        /// Error message describing what went wrong.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for returning success messages in delivery person API responses.
    /// </summary>
    public class DeliverySuccessResponse
    {
        /// <summary>
        /// Success message describing the completed operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}