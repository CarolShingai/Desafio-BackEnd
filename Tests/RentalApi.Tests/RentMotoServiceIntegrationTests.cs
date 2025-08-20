using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RentalApi.Application.Services;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using Xunit;

namespace Tests.RentalApi.Tests
{
    /// <summary>
    /// Testes de integração que simulam cenários reais de uso do sistema de locação
    /// </summary>
    public class RentMotoServiceIntegrationTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public RentMotoServiceIntegrationTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task FullRentalFlow_ShouldWork_WithEarlyReturn()
        {
            // Cenário: Entregador aluga moto por 7 dias e devolve 2 dias antes
            using var scope = _fixture.ServiceProvider.CreateScope();
            var rentService = scope.ServiceProvider.GetRequiredService<IRentMotoService>();
            var deliveryRepo = scope.ServiceProvider.GetRequiredService<IDeliveryPersonRepository>();

            // 1. Criar entregador com CNH categoria A
            var deliveryPerson = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid().ToString(),
                Name = "João Silva",
                Cnpj = "12345678901234",
                BirthDate = "1990-01-01",
                Cnh = "123456789",
                CnhType = "A",
                CnhImage = "path/to/image"
            };
            await deliveryRepo.AddDeliveryPersonAsync(deliveryPerson);

            // 2. Criar locação de 7 dias
            var rental = await rentService.CreateRentalAsync(
                deliveryPerson.Identifier, 
                "123", 
                7
            );

            // Verificações da criação
            Assert.NotNull(rental);
            Assert.Equal(7, rental.PlanDays);
            Assert.Equal(30m, rental.DailyRate);
            Assert.Equal(210m, rental.TotalCost);
            Assert.True(rental.StartDate > DateTime.UtcNow.Date);
            Assert.NotNull(rental.ExpectedReturnDate);

            // 3. Simular devolução 2 dias antes do prazo
            var earlyReturnDate = rental.ExpectedReturnDate.Value.AddDays(-2);
            var simulatedValue = await rentService.SimulateReturnValueAsync(rental.RentId, earlyReturnDate);
            
            // Verificar simulação (multa de 20% sobre dias não utilizados)
            // Base: R$210, Dias não utilizados: 2 × R$30 = R$60, Multa: R$60 × 20% = R$12
            // Total: R$210 + R$12 = R$222
            Assert.Equal(222m, simulatedValue);

            // 4. Informar devolução real
            var returnedRental = await rentService.InformReturnDateAsync(rental.RentId, earlyReturnDate);
            Assert.Equal(earlyReturnDate, returnedRental.ActualReturnDate);
            Assert.Equal(222m, returnedRental.TotalCost);

            // 5. Consultar valor final
            var finalValue = await rentService.GetFinalRentalValueAsync(rental.RentId);
            Assert.Equal(222m, finalValue);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task FullRentalFlow_ShouldWork_WithLateReturn()
        {
            // Cenário: Entregador aluga moto por 15 dias e devolve 3 dias depois
            using var scope = _fixture.ServiceProvider.CreateScope();
            var rentService = scope.ServiceProvider.GetRequiredService<IRentMotoService>();
            var deliveryRepo = scope.ServiceProvider.GetRequiredService<IDeliveryPersonRepository>();

            // 1. Criar entregador
            var deliveryPerson = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Identificador = Guid.NewGuid().ToString(),
                Nome = "Maria Santos",
                Cnpj = "98765432109876",
                DataNascimento = DateTime.Now.AddYears(-25),
                NumeroCnh = "987654321",
                CnhType = "A+B",
                ImagemCnh = "path/to/image2"
            };
            await deliveryRepo.AddDeliveryPersonAsync(deliveryPerson);

            // 2. Criar locação de 15 dias
            var rental = await rentService.CreateRentalAsync(
                deliveryPerson.Identificador, 
                "456", 
                15
            );

            // Verificações da criação
            Assert.Equal(15, rental.PlanDays);
            Assert.Equal(28m, rental.DailyRate);
            Assert.Equal(420m, rental.TotalCost);

            // 3. Simular devolução 3 dias após o prazo
            var lateReturnDate = rental.ExpectedReturnDate.Value.AddDays(3);
            var simulatedValue = await rentService.SimulateReturnValueAsync(rental.RentId, lateReturnDate);
            
            // Verificar simulação (R$50 por dia de atraso)
            // Base: R$420, Multa atraso: 3 × R$50 = R$150
            // Total: R$420 + R$150 = R$570
            Assert.Equal(570m, simulatedValue);

            // 4. Informar devolução real
            var returnedRental = await rentService.InformReturnDateAsync(rental.RentId, lateReturnDate);
            Assert.Equal(570m, returnedRental.TotalCost);

            // 5. Consultar valor final
            var finalValue = await rentService.GetFinalRentalValueAsync(rental.RentId);
            Assert.Equal(570m, finalValue);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task FullRentalFlow_ShouldWork_WithOnTimeReturn()
        {
            // Cenário: Entregador aluga moto por 30 dias e devolve no prazo
            using var scope = _fixture.ServiceProvider.CreateScope();
            var rentService = scope.ServiceProvider.GetRequiredService<IRentMotoService>();
            var deliveryRepo = scope.ServiceProvider.GetRequiredService<IDeliveryPersonRepository>();

            // 1. Criar entregador
            var deliveryPerson = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Identificador = Guid.NewGuid().ToString(),
                Nome = "Carlos Pereira",
                Cnpj = "11111111111111",
                DataNascimento = DateTime.Now.AddYears(-35),
                NumeroCnh = "111111111",
                CnhType = "A",
                ImagemCnh = "path/to/image3"
            };
            await deliveryRepo.AddDeliveryPersonAsync(deliveryPerson);

            // 2. Criar locação de 30 dias
            var rental = await rentService.CreateRentalAsync(
                deliveryPerson.Identificador, 
                "789", 
                30
            );

            // Verificações da criação
            Assert.Equal(30, rental.PlanDays);
            Assert.Equal(22m, rental.DailyRate);
            Assert.Equal(660m, rental.TotalCost);

            // 3. Simular devolução no prazo exato
            var onTimeReturnDate = rental.ExpectedReturnDate.Value;
            var simulatedValue = await rentService.SimulateReturnValueAsync(rental.RentId, onTimeReturnDate);
            
            // Verificar simulação (sem multas)
            Assert.Equal(660m, simulatedValue);

            // 4. Informar devolução real
            var returnedRental = await rentService.InformReturnDateAsync(rental.RentId, onTimeReturnDate);
            Assert.Equal(660m, returnedRental.TotalCost);

            // 5. Consultar valor final
            var finalValue = await rentService.GetFinalRentalValueAsync(rental.RentId);
            Assert.Equal(660m, finalValue);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task FullRentalFlow_ShouldWork_WithLongTermPlanEarlyReturn()
        {
            // Cenário: Entregador aluga moto por 50 dias e devolve 10 dias antes (sem multa)
            using var scope = _fixture.ServiceProvider.CreateScope();
            var rentService = scope.ServiceProvider.GetRequiredService<IRentMotoService>();
            var deliveryRepo = scope.ServiceProvider.GetRequiredService<IDeliveryPersonRepository>();

            // 1. Criar entregador
            var deliveryPerson = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Identificador = Guid.NewGuid().ToString(),
                Nome = "Ana Costa",
                Cnpj = "22222222222222",
                DataNascimento = DateTime.Now.AddYears(-28),
                NumeroCnh = "222222222",
                CnhType = "A",
                ImagemCnh = "path/to/image4"
            };
            await deliveryRepo.AddDeliveryPersonAsync(deliveryPerson);

            // 2. Criar locação de 50 dias
            var rental = await rentService.CreateRentalAsync(
                deliveryPerson.Identificador, 
                "999", 
                50
            );

            // Verificações da criação
            Assert.Equal(50, rental.PlanDays);
            Assert.Equal(18m, rental.DailyRate);
            Assert.Equal(900m, rental.TotalCost);

            // 3. Simular devolução 10 dias antes (planos longos não têm multa)
            var earlyReturnDate = rental.ExpectedReturnDate.Value.AddDays(-10);
            var simulatedValue = await rentService.SimulateReturnValueAsync(rental.RentId, earlyReturnDate);
            
            // Verificar simulação (sem multa para planos de 50 dias)
            Assert.Equal(900m, simulatedValue);

            // 4. Informar devolução real
            var returnedRental = await rentService.InformReturnDateAsync(rental.RentId, earlyReturnDate);
            Assert.Equal(900m, returnedRental.TotalCost);

            // 5. Consultar valor final
            var finalValue = await rentService.GetFinalRentalValueAsync(rental.RentId);
            Assert.Equal(900m, finalValue);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task RentalCreation_ShouldFail_WithInvalidCnh()
        {
            // Cenário: Entregador com CNH categoria B tenta alugar moto
            using var scope = _fixture.ServiceProvider.CreateScope();
            var rentService = scope.ServiceProvider.GetRequiredService<IRentMotoService>();
            var deliveryRepo = scope.ServiceProvider.GetRequiredService<IDeliveryPersonRepository>();

            // 1. Criar entregador com CNH categoria B
            var deliveryPerson = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Identificador = Guid.NewGuid().ToString(),
                Nome = "Pedro Oliveira",
                Cnpj = "33333333333333",
                DataNascimento = DateTime.Now.AddYears(-40),
                NumeroCnh = "333333333",
                CnhType = "B", // CNH inválida para motos
                ImagemCnh = "path/to/image5"
            };
            await deliveryRepo.AddDeliveryPersonAsync(deliveryPerson);

            // 2. Tentar criar locação (deve falhar)
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => rentService.CreateRentalAsync(deliveryPerson.Identificador, "123", 7)
            );

            Assert.Equal("Somente entregadores habilitados na categoria A podem efetuar uma locação", exception.Message);
        }
    }

    /// <summary>
    /// Fixture para configurar dependências de teste
    /// </summary>
    public class TestFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public TestFixture()
        {
            var services = new ServiceCollection();
            
            // Configurar serviços de teste (usando implementações em memória)
            services.AddScoped<IRentMotoService, RentMotoService>();
            // services.AddScoped<IRentMotoRepository, InMemoryRentMotoRepository>();
            // services.AddScoped<IDeliveryPersonRepository, InMemoryDeliveryPersonRepository>();
            
            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            ServiceProvider?.Dispose();
        }
    }
}
