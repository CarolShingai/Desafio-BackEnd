using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Domain.ValueObjects;

namespace RentalApi.Application.Services
{
	public class RentMotoService : IRentMotoService
	{
		private readonly IRentMotoRepository _rentMotoRepository;
		private readonly IDeliveryPersonRepository _deliveryRepo;
		public RentMotoService(IRentMotoRepository rentMotoRepository, IDeliveryPersonRepository deliveryPersonRepository)
		{
			_rentMotoRepository = rentMotoRepository;
		}
		public
	}
}
