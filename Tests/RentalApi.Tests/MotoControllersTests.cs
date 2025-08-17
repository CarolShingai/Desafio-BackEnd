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
			Identificador = "test-post",
			Ano = 2025,
			Modelo = "Test Model Post",
			Placa = "AAA-1111"
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
            Identificador = "all-motos-test",
            Ano = 2022,
            Modelo = "Test All Model",
            Placa = "AAA-0002"
        };
        await _client.PostAsJsonAsync("/api/MotoControllers", createRequest);

        // Busca todas as motos
        var response = await _client.GetAsync("/api/MotoControllers");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var motos = await response.Content.ReadFromJsonAsync<List<MotoResponse>>();
        Assert.NotNull(motos);
        Assert.Contains(motos, m => m.Identificador == "all-motos-test");
    }

	[Fact]
	public async Task GetMotoById_ShouldReturnOk_WhenExists()
	{
        var createRequest = new CreateMotoRequest
        {
            Identificador = "id-test",
            Ano = 2023,
            Modelo = "Test Id Model",
            Placa = "ZZZ-0002"
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
            Identificador = "update-test",
            Ano = 2025,
            Modelo = "Test Model",
            Placa = "UPD-0001"
        };
        await _client.PostAsJsonAsync("/api/MotoControllers", createRequest);

        var updateRequest = new UpdateMotoPlacaRequest { Placa = "NEW-1234" };
        var response = await _client.PutAsJsonAsync("/api/MotoControllers/update-test/placa", updateRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

	[Fact]
	public async Task UpdateMotoPlaca_ShouldReturnBadRequest_WhenInvalid()
	{
        var createRequest = new CreateMotoRequest
        {
            Identificador = "invalid-update-test",
            Ano = 2020,
            Modelo = "Test Model",
            Placa = "XXX-0001"
        };
        await _client.PostAsJsonAsync("/api/MotoControllers", createRequest);

		var updateRequest = new UpdateMotoPlacaRequest { Placa = "" };
		var response = await _client.PutAsJsonAsync("/api/MotoControllers/not-exist/placa", updateRequest);
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

    [Fact]
    public async Task DeleteMoto_ShouldReturnNoContent_WhenSuccess()
    {
        var createRequest = new CreateMotoRequest
        {
            Identificador = "delete-test",
            Ano = 2025,
            Modelo = "Delete Model",
            Placa = "DEL-0001"
        };
        await _client.PostAsJsonAsync("/api/MotoControllers", createRequest);

        var response = await _client.DeleteAsync("/api/MotoControllers/delete-test");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
