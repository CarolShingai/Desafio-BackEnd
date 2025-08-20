using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RentalApi.Application.DTOs;
using RentalApi.Infrastructure.Data;
using Xunit;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<RentalDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<RentalDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb" + Guid.NewGuid());
            });
        });
    }
}

public class MotoControllersTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MotoControllersTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

	[Fact]
	public async Task PostMoto_ShouldReturnCreated()
	{
		var request = new CreateMotoRequest
		{
			Identifier = "test-post",
			Year = 2025,
			MotorcycleModel = "Test Model Post",
			LicensePlate = "AAA-1111"
		};

		var response = await _client.PostAsJsonAsync("/api/MotoControllers", request);
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
	}

	[Fact]
	public async Task GetAllMotos_ShouldReturnOkAndContainMoto()
    {
        // Cria uma moto única para este teste
        var createRequest = new CreateMotoRequest
        {
            Identifier = "all-motos-test",
            Year = 2022,
            MotorcycleModel = "Test All Model",
            LicensePlate = "AAA-0002"
        };
        await _client.PostAsJsonAsync("/api/MotoControllers", createRequest);

        // Busca todas as motos
        var response = await _client.GetAsync("/api/MotoControllers");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var motos = await response.Content.ReadFromJsonAsync<List<MotoResponse>>();
        Assert.NotNull(motos);
        Assert.Contains(motos, m => m.Identifier == "all-motos-test");
    }

	[Fact]
	public async Task GetMotoById_ShouldReturnOk_WhenExists()
	{
        var createRequest = new CreateMotoRequest
        {
            Identifier = "id-test",
            Year = 2023,
            MotorcycleModel = "Test Id Model",
            LicensePlate = "ZZZ-0002"
        };

        await _client.PostAsJsonAsync("/api/MotoControllers", createRequest);
		var response = await _client.GetAsync("/api/MotoControllers/id-test");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task GetMotoById_ShouldReturnNotFound_WhenNotExists()
	{
		var response = await _client.GetAsync("/api/MotoControllers/not-exist");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
    public async Task UpdateMotoPlaca_ShouldReturnOk_WhenSuccess()
    {
        // Cria uma moto única para o teste
        var createRequest = new CreateMotoRequest
        {
            Identifier = "update-test",
            Year = 2025,
            MotorcycleModel = "Test Model",
            LicensePlate = "UPD-0001"
        };
        await _client.PostAsJsonAsync("/api/MotoControllers", createRequest);

        var updateRequest = new UpdateMotoPlacaRequest { LicensePlate = "NEW-1234" };
        var response = await _client.PutAsJsonAsync("/api/MotoControllers/update-test/placa", updateRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

	[Fact]
	public async Task UpdateMotoPlaca_ShouldReturnBadRequest_WhenInvalid()
	{
        var createRequest = new CreateMotoRequest
        {
            Identifier = "invalid-update-test",
            Year = 2020,
            MotorcycleModel = "Test Model",
            LicensePlate = "XXX-0001"
        };
        await _client.PostAsJsonAsync("/api/MotoControllers", createRequest);

		var updateRequest = new UpdateMotoPlacaRequest { LicensePlate = "" };
		var response = await _client.PutAsJsonAsync("/api/MotoControllers/not-exist/placa", updateRequest);
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

    [Fact]
    public async Task DeleteMoto_ShouldReturnNoContent_WhenSuccess()
    {
        var createRequest = new CreateMotoRequest
        {
            Identifier = "delete-test",
            Year = 2025,
            MotorcycleModel = "Delete Model",
            LicensePlate = "DEL-0001"
        };
        await _client.PostAsJsonAsync("/api/MotoControllers", createRequest);

        var response = await _client.DeleteAsync("/api/MotoControllers/delete-test");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
