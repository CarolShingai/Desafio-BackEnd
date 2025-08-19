using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Application.Services
{
	public class RentMotoService : IRentMotoService
	{
		private readonly IRentMotoRepository _rentMotoRepository;
		public RentMotoService(IRentMotoRepository rentMotoRepository)
		{
			_rentMotoRepository = rentMotoRepository;
		}
		public 
	}
}
