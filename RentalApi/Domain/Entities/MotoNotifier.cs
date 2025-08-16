
namespace RentalApi.Domain.Entities
{
	public class MotoNotifier
	{
		public Guid Id { get; set; }
		public string MotorcycleId { get; set; } = string.Empty;
		public int Year { get; set; }
		public string Model { get; set; } = string.Empty;
		public string LicensePlate { get; set; } = string.Empty;
		public DateTime NotifiedAt { get; set; }
		public string Message { get; set; } = string.Empty;

		public MotoNotifier() {}

		public MotoNotifier(string motorcycleId, int year, string model, string licensePlate)
		{
			Id = Guid.NewGuid();
			MotorcycleId = motorcycleId;
			Year = year;
			Model = model;
			LicensePlate = licensePlate;
			NotifiedAt = DateTime.UtcNow;
			Message = $"Moto de {year} foi registrada: {model} - {licensePlate}";
		}
	}
}
