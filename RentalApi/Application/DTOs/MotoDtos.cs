namespace RentalApi.Application.DTOs
{
    /// <summary>
    /// DTO for creating a new motorcycle.
    /// </summary>
    public class CreateMotoRequest
    {
        /// <summary>
        /// Unique identifier for the motorcycle.
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
    }

    /// <summary>
    /// DTO for updating the license plate of a motorcycle.
    /// </summary>
    public class UpdateMotoPlacaRequest
    {
        /// <summary>
        /// New license plate for the motorcycle.
        /// </summary>
        public string Placa { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for returning motorcycle data in responses.
    /// </summary>
    public class MotoResponse
    {
        /// <summary>
        /// Unique identifier for the motorcycle.
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