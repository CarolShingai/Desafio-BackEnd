 using System.Text.Json.Serialization;

namespace RentalApi.Application.DTOs
{
    /// <summary>
    /// DTO used to create a new motorcycle via the API.
    /// </summary>
    public class CreateMotoRequest
    {
        /// <summary>
        /// Unique business identifier for the motorcycle (required).
        /// </summary>
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// Manufacturing year of the motorcycle (must be between 2000 and current year).
        /// </summary>
        [JsonPropertyName("ano")]
        public int Year { get; set; }

        /// <summary>
        /// Model name of the motorcycle (e.g., "Honda CG 160").
        /// </summary>
        [JsonPropertyName("modelo")]
        public string MotorcycleModel { get; set; } = string.Empty;

        /// <summary>
        /// License plate in Brazilian format (e.g., "ABC-1234").
        /// </summary>
        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO used to update the license plate of an existing motorcycle.
    /// </summary>
    public class UpdateMotoPlacaRequest
    {
        /// <summary>
        /// New license plate for the motorcycle (must be unique and valid format).
        /// </summary>
        public string LicensePlate { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO used to return motorcycle data in API responses.
    /// </summary>
    public class MotoResponse
    {
        /// <summary>
        /// Unique business identifier for the motorcycle.
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
    /// DTO used to return error messages in API responses.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Error message describing the problem.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO used to return success messages in API responses.
    /// </summary>
    public class SuccessResponse
    {
        /// <summary>
        /// Success message describing the completed operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}