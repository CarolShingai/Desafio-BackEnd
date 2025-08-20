using System;
using System.Threading.Tasks;
using Moq;
using RentalApi.Application.Services;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using Xunit;

namespace Tests.RentalApi.Tests
{
    public class RentMotoServiceBusinessRulesTests
    {
        private readonly Mock<IRentMotoRepository> _mockRentRepo;
        private readonly Mock<IDeliveryPersonRepository> _mockDeliveryRepo;
        private readonly Mock<IMotoRepository> _mockMotoRepo;
        private readonly RentMotoService _service;

        public RentMotoServiceBusinessRulesTests()
        {
            _mockRentRepo = new Mock<IRentMotoRepository>();
            _mockDeliveryRepo = new Mock<IDeliveryPersonRepository>();
            _mockMotoRepo = new Mock<IMotoRepository>();
            _service = new RentMotoService(_mockRentRepo.Object, _mockDeliveryRepo.Object, _mockMotoRepo.Object);
        }

        #region CreateRentalAsync Tests

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateRentalAsync_ShouldCreateRental_WhenEntregadorHasCnhA()
        {
            // Arrange
            var deliveryPersonId = Guid.NewGuid().ToString();
            var motoId = "123";
            var planDays = 7;
            var deliveryPerson = new DeliveryPerson { Id = Guid.Parse(deliveryPersonId), CnhType = "A" };
            var moto = new Moto 
            { 
                Id = 123,
                Identifier = motoId,
                Year = 2024,
                MotorcycleModel = "Test Model",
                LicensePlate = "ABC-1234"
            };
            
            _mockDeliveryRepo.Setup(r => r.FindDeliveryPersonByIdentifierAsync(deliveryPersonId))
                .ReturnsAsync(deliveryPerson);
            _mockMotoRepo.Setup(r => r.FindByMotoIdentifierAsync(motoId))
                .ReturnsAsync(moto);
            _mockRentRepo.Setup(r => r.AddRentalAsync(It.IsAny<RentMoto>()))
                .ReturnsAsync((RentMoto r) => r);

            // Act
            var result = await _service.CreateRentalAsync(deliveryPersonId, motoId, planDays);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(123, result.MotoId);
            Assert.Equal(Guid.Parse(deliveryPersonId), result.DeliveryPersonId);
            Assert.Equal(7, result.PlanDays);
            Assert.Equal(30m, result.DailyRate); // Plano 7 dias = R$30/dia
            Assert.Equal(210m, result.TotalCost); // 7 × 30 = 210
            Assert.True(result.StartDate > DateTime.UtcNow.Date); // Início é D+1
            Assert.NotNull(result.ExpectedReturnDate);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateRentalAsync_ShouldCreateRental_WhenEntregadorHasCnhAB()
        {
            // Arrange
            var deliveryPersonId = Guid.NewGuid().ToString();
            var motoId = "123";
            var planDays = 15;
            var deliveryPerson = new DeliveryPerson { Id = Guid.Parse(deliveryPersonId), CnhType = "A+B" };
            var moto = new Moto 
            { 
                Id = 123,
                Identifier = motoId,
                Year = 2024,
                MotorcycleModel = "Test Model",
                LicensePlate = "ABC-1234"
            };
            
            _mockDeliveryRepo.Setup(r => r.FindDeliveryPersonByIdentifierAsync(deliveryPersonId))
                .ReturnsAsync(deliveryPerson);
            _mockMotoRepo.Setup(r => r.FindByMotoIdentifierAsync(motoId))
                .ReturnsAsync(moto);
            _mockRentRepo.Setup(r => r.AddRentalAsync(It.IsAny<RentMoto>()))
                .ReturnsAsync((RentMoto r) => r);

            // Act
            var result = await _service.CreateRentalAsync(deliveryPersonId, motoId, planDays);

            // Assert
            Assert.Equal(28m, result.DailyRate); // Plano 15 dias = R$28/dia
            Assert.Equal(420m, result.TotalCost); // 15 × 28 = 420
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateRentalAsync_ShouldThrow_WhenEntregadorNotFound()
        {
            // Arrange
            var deliveryPersonId = Guid.NewGuid().ToString();
            var motoId = "123";
            var planDays = 7;
            
            _mockDeliveryRepo.Setup(r => r.FindDeliveryPersonByIdentifierAsync(deliveryPersonId))
                .ReturnsAsync((DeliveryPerson)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateRentalAsync(deliveryPersonId, motoId, planDays)
            );
            Assert.Equal("Delivery person not found.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateRentalAsync_ShouldThrow_WhenEntregadorHasInvalidCnh()
        {
            // Arrange
            var deliveryPersonId = Guid.NewGuid().ToString();
            var motoId = "123";
            var planDays = 7;
            var deliveryPerson = new DeliveryPerson { Id = Guid.Parse(deliveryPersonId), CnhType = "B" };
            
            _mockDeliveryRepo.Setup(r => r.FindDeliveryPersonByIdentifierAsync(deliveryPersonId))
                .ReturnsAsync(deliveryPerson);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateRentalAsync(deliveryPersonId, motoId, planDays)
            );
            Assert.Equal("Only drivers with category A license can rent a motorcycle.", exception.Message);
        }

        [Theory]
        [InlineData(7, 30, 210)]    // 7 dias × R$30 = R$210
        [InlineData(15, 28, 420)]   // 15 dias × R$28 = R$420
        [InlineData(30, 22, 660)]   // 30 dias × R$22 = R$660
        [InlineData(45, 20, 900)]   // 45 dias × R$20 = R$900
        [InlineData(50, 18, 900)]   // 50 dias × R$18 = R$900
        [Trait("Category", "Unit")]
        public async Task CreateRentalAsync_ShouldCalculateCorrectCost_ForAllPlans(int planDays, int expectedDailyRateInt, int expectedTotalCostInt)
        {
            // Arrange
            var deliveryPersonId = Guid.NewGuid().ToString();
            var motoId = "123";
            var deliveryPerson = new DeliveryPerson { Id = Guid.Parse(deliveryPersonId), CnhType = "A" };
            var moto = new Moto 
            { 
                Id = 123,
                Identifier = motoId,
                Year = 2024,
                MotorcycleModel = "Test Model",
                LicensePlate = "ABC-1234"
            };
            
            _mockDeliveryRepo.Setup(r => r.FindDeliveryPersonByIdentifierAsync(deliveryPersonId))
                .ReturnsAsync(deliveryPerson);
            _mockMotoRepo.Setup(r => r.FindByMotoIdentifierAsync(motoId))
                .ReturnsAsync(moto);
            _mockRentRepo.Setup(r => r.AddRentalAsync(It.IsAny<RentMoto>()))
                .ReturnsAsync((RentMoto r) => r);

            // Act
            var result = await _service.CreateRentalAsync(deliveryPersonId, motoId, planDays);

            // Assert
            Assert.Equal(expectedDailyRateInt, (int)result.DailyRate);
            Assert.Equal(expectedTotalCostInt, (int)result.TotalCost);
        }

        #endregion

        #region SimulateReturnValueAsync Tests - Regras de Multa

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SimulateReturnValueAsync_ShouldCalculateEarlyReturnPenalty_For7DaysPlan()
        {
            // Arrange - Plano 7 dias, devolução 2 dias antes
            var rentId = "rent-123";
            var startDate = DateTime.UtcNow.Date.AddDays(1);
            var expectedEndDate = startDate.AddDays(7);
            var returnDate = expectedEndDate.AddDays(-2); // 2 dias antes

            var rental = new RentMoto
            {
                RentId = rentId,
                StartDate = startDate,
                ExpectedReturnDate = expectedEndDate,
                PlanDays = 7,
                DailyRate = 30m,
                TotalCost = 210m
            };

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync(rental);

            // Act
            var result = await _service.SimulateReturnValueAsync(rentId, returnDate);

            // Assert
            // Custo base: 7 × R$30 = R$210
            // Dias não utilizados: 2 × R$30 = R$60
            // Multa 20%: R$60 × 0.2 = R$12
            // Total: R$210 + R$12 = R$222
            Assert.Equal(222m, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SimulateReturnValueAsync_ShouldCalculateEarlyReturnPenalty_For15DaysPlan()
        {
            // Arrange - Plano 15 dias, devolução 3 dias antes
            var rentId = "rent-123";
            var startDate = DateTime.UtcNow.Date.AddDays(1);
            var expectedEndDate = startDate.AddDays(15);
            var returnDate = expectedEndDate.AddDays(-3); // 3 dias antes

            var rental = new RentMoto
            {
                RentId = rentId,
                StartDate = startDate,
                ExpectedReturnDate = expectedEndDate,
                PlanDays = 15,
                DailyRate = 28m,
                TotalCost = 420m
            };

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync(rental);

            // Act
            var result = await _service.SimulateReturnValueAsync(rentId, returnDate);

            // Assert
            // Custo base: 15 × R$28 = R$420
            // Dias não utilizados: 3 × R$28 = R$84
            // Multa 40%: R$84 × 0.4 = R$33.60
            // Total: R$420 + R$33.60 = R$453.60
            Assert.Equal(453.60m, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SimulateReturnValueAsync_ShouldNotChargePenalty_For30DaysPlan()
        {
            // Arrange - Plano 30 dias, devolução 5 dias antes (sem multa)
            var rentId = "rent-123";
            var startDate = DateTime.UtcNow.Date.AddDays(1);
            var expectedEndDate = startDate.AddDays(30);
            var returnDate = expectedEndDate.AddDays(-5); // 5 dias antes

            var rental = new RentMoto
            {
                RentId = rentId,
                StartDate = startDate,
                ExpectedReturnDate = expectedEndDate,
                PlanDays = 30,
                DailyRate = 22m,
                TotalCost = 660m
            };

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync(rental);

            // Act
            var result = await _service.SimulateReturnValueAsync(rentId, returnDate);

            // Assert
            // Custo base: 30 × R$22 = R$660 (sem multa para planos 30+ dias)
            Assert.Equal(660m, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SimulateReturnValueAsync_ShouldChargeLateReturnFee_ForAllPlans()
        {
            // Arrange - Plano 7 dias, devolução 3 dias depois
            var rentId = "rent-123";
            var startDate = DateTime.UtcNow.Date.AddDays(1);
            var expectedEndDate = startDate.AddDays(7);
            var returnDate = expectedEndDate.AddDays(3); // 3 dias depois

            var rental = new RentMoto
            {
                RentId = rentId,
                StartDate = startDate,
                ExpectedReturnDate = expectedEndDate,
                PlanDays = 7,
                DailyRate = 30m,
                TotalCost = 210m
            };

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync(rental);

            // Act
            var result = await _service.SimulateReturnValueAsync(rentId, returnDate);

            // Assert
            // Custo base: 7 × R$30 = R$210
            // Multa atraso: 3 × R$50 = R$150
            // Total: R$210 + R$150 = R$360
            Assert.Equal(360m, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SimulateReturnValueAsync_ShouldReturnBaseCost_WhenReturnOnTime()
        {
            // Arrange - Devolução exatamente no prazo
            var rentId = "rent-123";
            var startDate = DateTime.UtcNow.Date.AddDays(1);
            var expectedEndDate = startDate.AddDays(7);
            var returnDate = expectedEndDate; // Exatamente no prazo

            var rental = new RentMoto
            {
                RentId = rentId,
                StartDate = startDate,
                ExpectedReturnDate = expectedEndDate,
                PlanDays = 7,
                DailyRate = 30m,
                TotalCost = 210m
            };

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync(rental);

            // Act
            var result = await _service.SimulateReturnValueAsync(rentId, returnDate);

            // Assert
            // Apenas custo base: 7 × R$30 = R$210 (sem multas)
            Assert.Equal(210m, result);
        }

        #endregion

        #region InformReturnDateAsync Tests

        [Fact]
        [Trait("Category", "Unit")]
        public async Task InformReturnDateAsync_ShouldUpdateRentalAndCalculateFinalValue()
        {
            // Arrange
            var rentId = "rent-123";
            var returnDate = DateTime.UtcNow.Date.AddDays(10);
            var rental = new RentMoto
            {
                RentId = rentId,
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                ExpectedReturnDate = DateTime.UtcNow.Date.AddDays(8),
                PlanDays = 7,
                DailyRate = 30m,
                TotalCost = 210m,
                ActualReturnDate = null
            };

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync(rental);
            _mockRentRepo.Setup(r => r.UpdateRentAsync(It.IsAny<RentMoto>()))
                .ReturnsAsync((RentMoto r) => r);

            // Act
            var result = await _service.InformReturnDateAsync(rentId, returnDate);

            // Assert
            Assert.Equal(returnDate, result.ActualReturnDate);
            Assert.True(result.TotalCost > 210m); // Deve ter multa por atraso
            _mockRentRepo.Verify(r => r.UpdateRentAsync(It.IsAny<RentMoto>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task InformReturnDateAsync_ShouldThrow_WhenReturnDateAlreadyInformed()
        {
            // Arrange
            var rentId = "rent-123";
            var returnDate = DateTime.UtcNow.Date.AddDays(7);
            var rental = new RentMoto
            {
                RentId = rentId,
                ActualReturnDate = DateTime.UtcNow.Date.AddDays(6) // Já tem data de devolução
            };

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync(rental);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.InformReturnDateAsync(rentId, returnDate)
            );
            Assert.Equal("Return date has already been provided for this rental", exception.Message);
        }

        #endregion

        #region GetFinalRentalValueAsync Tests

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetFinalRentalValueAsync_ShouldReturnFinalValue_WhenReturnDateInformed()
        {
            // Arrange
            var rentId = "rent-123";
            var rental = new RentMoto
            {
                RentId = rentId,
                TotalCost = 250m,
                ActualReturnDate = DateTime.UtcNow.Date.AddDays(7)
            };

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync(rental);

            // Act
            var result = await _service.GetFinalRentalValueAsync(rentId);

            // Assert
            Assert.Equal(250m, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetFinalRentalValueAsync_ShouldThrow_WhenReturnDateNotInformed()
        {
            // Arrange
            var rentId = "rent-123";
            var rental = new RentMoto
            {
                RentId = rentId,
                TotalCost = 210m,
                ActualReturnDate = null // Data de devolução não informada
            };

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync(rental);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.GetFinalRentalValueAsync(rentId)
            );
            Assert.Equal("Return date has not been provided yet", exception.Message);
        }

        #endregion

        #region Error Cases

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateRentalAsync_ShouldThrow_WhenInvalidPlanDays()
        {
            // Arrange
            var deliveryPersonId = Guid.NewGuid().ToString();
            var motoId = "123";
            var planDays = 10; // Plano inválido
            var deliveryPerson = new DeliveryPerson { Id = Guid.Parse(deliveryPersonId), CnhType = "A" };
            
            _mockDeliveryRepo.Setup(r => r.FindDeliveryPersonByIdentifierAsync(deliveryPersonId))
                .ReturnsAsync(deliveryPerson);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateRentalAsync(deliveryPersonId, motoId, planDays)
            );
            Assert.Contains("Invalid rental plan", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SimulateReturnValueAsync_ShouldThrow_WhenRentalNotFound()
        {
            // Arrange
            var rentId = "inexistent-rent";
            var returnDate = DateTime.UtcNow.Date.AddDays(7);

            _mockRentRepo.Setup(r => r.FindRentalByIdAsync(rentId)).ReturnsAsync((RentMoto)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.SimulateReturnValueAsync(rentId, returnDate)
            );
            Assert.Equal("Rental not found", exception.Message);
        }

        #endregion
    }
}
