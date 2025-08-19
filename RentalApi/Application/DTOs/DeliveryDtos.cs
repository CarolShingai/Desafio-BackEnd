namespace RentalApi.Application.DTOs
{
    public class CreateDeliveryPersonRequest
    {
        public string Identificador { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string data_nascimento { get; set; } = string.Empty;
        public string numero_cnh { get; set; } = string.Empty;
        public string tipo_cnh { get; set; } = string.Empty;
        public string imagem_cnh { get; set; } = string.Empty;
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