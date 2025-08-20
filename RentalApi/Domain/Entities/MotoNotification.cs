
namespace RentalApi.Domain.Entities
{
	public class MotoNotification
	{
		public Guid Id { get; set; }
		public int MotorcycleId { get; set; }
		public string MotorcycleIdentifier { get; set; } = string.Empty;
		public int Year { get; set; }
		public string Model { get; set; } = string.Empty;
		public string LicensePlate { get; set; } = string.Empty;
		public DateTime NotifiedAt { get; set; }
		public string Message { get; set; } = string.Empty;

		public MotoNotification() {}

		public MotoNotification(int motorcycleId, string motorcycleIdentifier, int year, string model, string licensePlate)
		{
			Id = Guid.NewGuid();
			MotorcycleId = motorcycleId;
			MotorcycleIdentifier = motorcycleIdentifier;
			Year = year;
			Model = model;
			LicensePlate = licensePlate;
			NotifiedAt = DateTime.UtcNow;
			Message = $"Moto de {year} foi registrada: {model} - {licensePlate}";
		}
	}
}
