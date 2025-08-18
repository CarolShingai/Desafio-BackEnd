using System.Buffers.Text;

namespace RentalApi.Domain.Entities
{
    public class DeliveryPerson
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string Cnh { get; set; } = string.Empty;
        public string CnhType { get; set; } = string.Empty;
        public string CnhImage { get; set; } = string.Empty;
    }
}