namespace RentalApi.Domain.Entities
{
    public class Moto
    {
        public int Id { get; set; }
        public int Ano { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
    }
}