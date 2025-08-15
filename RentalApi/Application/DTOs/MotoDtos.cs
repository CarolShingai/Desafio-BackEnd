namespace RentalApi.Application.DTOs
{
    public class CreateMotoRequest
    {
        public string Identificador { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
    }
    public class UpdateMotoPlacaRequest
    {
        public string Placa { get; set; } = string.Empty;
    }
    public class MotoResponse
    {
        public string Identificador { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
    }
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class SuccessResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}