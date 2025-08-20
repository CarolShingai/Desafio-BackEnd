using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Infrastructure.Data;
using RentalApi.Infrastructure.Repositories;
using Xunit;

public class MotoRepositoryEFIntegrationTests
{
    private RentalDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<RentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Banco isolado por teste
            .Options;
        return new RentalDbContext(options);
    }

    [Fact]
    public async Task AddMotoAsync_And_FindMotoAllAsync_ShouldPersistAndReturnMoto()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        var moto = new Moto { Identifier = "int-001", Year = 2021, MotorcycleModel = "Honda", LicensePlate = "AAA-1111" };
        await repo.AddMotoAsync(moto);

        var allMotos = await repo.FindMotoAllAsync();
        Assert.Single(allMotos);
        Assert.Equal("int-001", allMotos[0].Identifier);
    }

    [Fact]
    public async Task FindByMotoIdentifierAsync_ShouldReturnCorrectMoto()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        var moto = new Moto { Identifier = "int-002", Year = 2022, MotorcycleModel = "Yamaha", LicensePlate = "BBB-2222" };
        await repo.AddMotoAsync(moto);

        var found = await repo.FindByMotoIdentifierAsync("int-002");
        Assert.NotNull(found);
        Assert.Equal("Yamaha", found.MotorcycleModel);
    }

    [Fact]
    public async Task FindByMotoLicenseAsync_ShouldReturnCorrectMoto()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        var moto = new Moto { Identifier = "int-003", Year = 2023, MotorcycleModel = "Suzuki", LicensePlate = "CCC-3333" };
        await repo.AddMotoAsync(moto);

        var found = await repo.FindByMotoLicenseAsync("CCC-3333");
        Assert.NotNull(found);
        Assert.Equal("int-003", found.Identifier);
    }

    [Fact]
    public async Task SearchMotosByLicenseAsync_ShouldReturnPartialMatches()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        await repo.AddMotoAsync(new Moto { Identifier = "int-004", Year = 2024, MotorcycleModel = "BMW", LicensePlate = "DDD-4444" });
        await repo.AddMotoAsync(new Moto { Identifier = "int-005", Year = 2025, MotorcycleModel = "BMW", LicensePlate = "DDD-5555" });

        var found = await repo.SearchMotosByLicenseAsync("DDD");
        Assert.Equal(2, found.Count);
    }

    [Fact]
    public async Task UpdateMotoLicenseAsync_ShouldUpdateLicense()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        await repo.AddMotoAsync(new Moto { Identifier = "int-006", Year = 2026, MotorcycleModel = "Kawasaki", LicensePlate = "EEE-6666" });

        var updated = await repo.UpdateMotoLicenseAsync("int-006", "NEW-6666");
        Assert.True(updated);

        var moto = await repo.FindByMotoLicenseAsync("NEW-6666");
        Assert.NotNull(moto);
        Assert.Equal("int-006", moto.Identifier);
    }

    [Fact]
    public async Task RemoveMotoAsync_ShouldRemoveMoto()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        await repo.AddMotoAsync(new Moto { Identifier = "int-007", Year = 2027, MotorcycleModel = "Harley", LicensePlate = "FFF-7777" });

        var removed = await repo.RemoveMotoAsync("int-007");
        Assert.True(removed);

        var moto = await repo.FindByMotoIdentifierAsync("int-007");
        Assert.Null(moto);
    }

    [Fact]
    public async Task RemoveMotoAsync_ShouldReturnFalse_WhenMotoDoesNotExist()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        var removed = await repo.RemoveMotoAsync("not-exist");
        Assert.False(removed);
    }
}