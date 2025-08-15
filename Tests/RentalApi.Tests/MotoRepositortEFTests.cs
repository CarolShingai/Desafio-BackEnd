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

        var moto = new Moto { Identificador = "int-001", Ano = 2021, Modelo = "Honda", Placa = "AAA-1111" };
        await repo.AddMotoAsync(moto);

        var allMotos = await repo.FindMotoAllAsync();
        Assert.Single(allMotos);
        Assert.Equal("int-001", allMotos[0].Identificador);
    }

    [Fact]
    public async Task FindByMotoIdentifierAsync_ShouldReturnCorrectMoto()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        var moto = new Moto { Identificador = "int-002", Ano = 2022, Modelo = "Yamaha", Placa = "BBB-2222" };
        await repo.AddMotoAsync(moto);

        var found = await repo.FindByMotoIdentifierAsync("int-002");
        Assert.NotNull(found);
        Assert.Equal("Yamaha", found.Modelo);
    }

    [Fact]
    public async Task FindByMotoLicenseAsync_ShouldReturnCorrectMoto()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        var moto = new Moto { Identificador = "int-003", Ano = 2023, Modelo = "Suzuki", Placa = "CCC-3333" };
        await repo.AddMotoAsync(moto);

        var found = await repo.FindByMotoLicenseAsync("CCC-3333");
        Assert.NotNull(found);
        Assert.Equal("int-003", found.Identificador);
    }

    [Fact]
    public async Task SearchMotosByLicenseAsync_ShouldReturnPartialMatches()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        await repo.AddMotoAsync(new Moto { Identificador = "int-004", Ano = 2024, Modelo = "BMW", Placa = "DDD-4444" });
        await repo.AddMotoAsync(new Moto { Identificador = "int-005", Ano = 2025, Modelo = "BMW", Placa = "DDD-5555" });

        var found = await repo.SearchMotosByLicenseAsync("DDD");
        Assert.Equal(2, found.Count);
    }

    [Fact]
    public async Task UpdateMotoLicenseAsync_ShouldUpdateLicense()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        await repo.AddMotoAsync(new Moto { Identificador = "int-006", Ano = 2026, Modelo = "Kawasaki", Placa = "EEE-6666" });

        var updated = await repo.UpdateMotoLicenseAsync("int-006", "NEW-6666");
        Assert.True(updated);

        var moto = await repo.FindByMotoLicenseAsync("NEW-6666");
        Assert.NotNull(moto);
        Assert.Equal("int-006", moto.Identificador);
    }

    [Fact]
    public async Task RemoveMotoAsync_ShouldRemoveMoto()
    {
        var context = GetDbContext();
        var repo = new MotoRepositoryEF(context);

        await repo.AddMotoAsync(new Moto { Identificador = "int-007", Ano = 2027, Modelo = "Harley", Placa = "FFF-7777" });

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