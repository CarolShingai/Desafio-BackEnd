using System.Text.Json.Serialization;

namespace RentalApi.Application.DTOs
{
    /// <summary>
    /// DTO for creating a new motorcycle.
    /// </summary>
    public class CreateMotoRequest
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("ano")]
        public int Year { get; set; }

        [JsonPropertyName("modelo")]
        public string MotorcycleModel { get; set; } = string.Empty;

        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating the license plate of a motorcycle.
    /// </summary>
    public class UpdateMotoPlacaRequest
    {
        /// <summary>
        /// New license plate for the motorcycle.
        /// </summary>
        public string LicensePlate { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for returning motorcycle data in responses.
    /// </summary>
    public class MotoResponse
    {
        /// <summary>
        /// Unique identifier for the motorcycle.
        /// </summary>
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// Manufacturing year of the motorcycle.
        /// </summary>
        [JsonPropertyName("ano")]
        public int Year { get; set; }

        /// <summary>
        /// Model name of the motorcycle.
        /// </summary>
        [JsonPropertyName("modelo")]
        public string MotorcycleModel { get; set; } = string.Empty;

        /// <summary>
        /// License plate of the motorcycle.
        /// </summary>
        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for returning error messages in API responses.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for returning success messages in API responses.
    /// </summary>
    public class SuccessResponse
    {
        /// <summary>
        /// Success message.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}