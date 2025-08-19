namespace RentalApi.Domain.Entities
{
    // precisa colocar outro identificador
    // mudar no repositóry, no service, no controller, no dto
    public class DeliveryPerson
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
}