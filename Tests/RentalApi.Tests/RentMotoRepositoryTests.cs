using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Infrastructure.Data;
using RentalApi.Infrastructure.Repositories;
using Xunit;

namespace RentalApi.Tests
{
	public class RentMotoRepositoryTests
	{
		private RentalDbContext GetInMemoryDbContext()
		{
			var options = new DbContextOptionsBuilder<RentalDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			return new RentalDbContext(options);
		}

		[Fact]
		[Trait("Category", "Unit")]
		public async Task AddRentalAsync_ShouldAddRental()
		{
			using var context = GetInMemoryDbContext();
			var repo = new RentMotoRepository(context);
			var rentMoto = new RentMoto { RentId = Guid.NewGuid().ToString() };

			var result = await repo.AddRentalAsync(rentMoto);

			Assert.NotNull(result);
			Assert.Equal(rentMoto.RentId, result.RentId);
		}

		[Fact]
		[Trait("Category", "Unit")]
		public async Task GetRentalByIdAsync_ShouldReturnRental_WhenExists()
		{
			using var context = GetInMemoryDbContext();
			var rentMoto = new RentMoto { RentId = "test-id" };
			context.RentMotos.Add(rentMoto);
			await context.SaveChangesAsync();
			var repo = new RentMotoRepository(context);

			var result = await repo.GetRentalByIdAsync("test-id");

			Assert.NotNull(result);
			Assert.Equal("test-id", result.RentId);
		}

		[Fact]
		[Trait("Category", "Unit")]
		public async Task GetRentalByIdAsync_ShouldReturnNull_WhenNotExists()
		{
			using var context = GetInMemoryDbContext();
			var repo = new RentMotoRepository(context);

			var result = await repo.GetRentalByIdAsync("not-exist");

			Assert.Null(result);
		}

		[Fact]
		[Trait("Category", "Unit")]
		public async Task UpdateRentAsync_ShouldUpdateRental()
		{
			using var context = GetInMemoryDbContext();
			var rentMoto = new RentMoto { RentId = "update-id" };
			context.RentMotos.Add(rentMoto);
			await context.SaveChangesAsync();
			var repo = new RentMotoRepository(context);

			rentMoto.RentId = "update-id";
			var updated = await repo.UpdateRentAsync(rentMoto);

			Assert.Equal("update-id", updated.RentId);
		}
	}
}
