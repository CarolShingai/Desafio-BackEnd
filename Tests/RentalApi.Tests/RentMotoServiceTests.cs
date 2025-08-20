using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using RentalApi.Application.Services;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using RentalApi.Domain.ValueObjects;
using Xunit;

namespace Tests.RentalApi.Tests
{
    public class RentMotoServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateRentalAsync_ShouldCreateRental_WhenValidData()
        {
            // Arrange
            var deliveryPersonId = Guid.NewGuid();
            var motoId = 1;
            var planDays = 7;
            var deliveryPerson = new DeliveryPerson { Id = deliveryPersonId, CnhType = "A" };
            var moto = new Moto { Id = motoId, IsRented = false };
            var mockRentRepo = new Mock<IRentMotoRepository>();
            var mockDeliveryRepo = new Mock<IDeliveryPersonRepository>();
            mockDeliveryRepo.Setup(r => r.FindDeliveryPersonByIdentifierAsync(deliveryPersonId.ToString())).ReturnsAsync(deliveryPerson);
            mockRentRepo.Setup(r => r.AddRentalAsync(It.IsAny<RentMoto>())).ReturnsAsync((RentMoto r) => r);
            var service = new RentMotoService(mockRentRepo.Object, mockDeliveryRepo.Object);

            // Act
            var result = await service.CreateRentalAsync(deliveryPersonId.ToString(), motoId.ToString(), planDays);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(motoId, result.MotoId);
            Assert.Equal(deliveryPersonId, result.DeliveryPersonId);
            Assert.Equal(planDays, result.PlanDays);
            Assert.Equal(30m, result.PricePerDay); // Plan de 7 dias tem rate de 30m
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateRentalAsync_ShouldThrow_WhenMotoAlreadyRented()
        {
            var deliveryPersonId = Guid.NewGuid();
            var motoId = 1;
            var planDays = 7;
            var deliveryPerson = new DeliveryPerson { Id = deliveryPersonId, CnhType = "A" };
            var moto = new Moto { Id = motoId, IsRented = true };
            var mockRentRepo = new Mock<IRentMotoRepository>();
            var mockDeliveryRepo = new Mock<IDeliveryPersonRepository>();
            mockDeliveryRepo.Setup(r => r.FindDeliveryPersonByIdentifierAsync(deliveryPersonId.ToString())).ReturnsAsync(deliveryPerson);
            var service = new RentMotoService(mockRentRepo.Object, mockDeliveryRepo.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateRentalAsync(deliveryPersonId.ToString(), motoId.ToString(), planDays));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task InformReturnDateAsync_ShouldUpdateRentalAndMoto()
        {
            var rentId = "rent-123";
            var motoId = 1;
            var deliveryPersonId = Guid.NewGuid();
            var rental = new RentMoto { RentId = rentId, MotoId = motoId, DeliveryPersonId = deliveryPersonId, StartDate = DateTime.UtcNow.Date, PlanDays = 7, DailyRate = 30 };
            var moto = new Moto { Id = motoId, IsRented = true };
            var mockRentRepo = new Mock<IRentMotoRepository>();
            var mockDeliveryRepo = new Mock<IDeliveryPersonRepository>();
            mockRentRepo.Setup(r => r.GetRentalByIdAsync(rentId)).ReturnsAsync(rental);
            mockRentRepo.Setup(r => r.UpdateRentAsync(rental)).ReturnsAsync(rental);
            var service = new RentMotoService(mockRentRepo.Object, mockDeliveryRepo.Object);

            var returnDate = DateTime.UtcNow.Date.AddDays(7);
            var result = await service.InformReturnDateAsync(rentId, returnDate);

            Assert.NotNull(result.ActualReturnDate);
            Assert.Equal(returnDate, result.ActualReturnDate.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetFinalRentalValueAsync_ShouldReturnCorrectValue()
        {
            var rentId = "rent-123";
            var planDays = 7;
            var startDate = DateTime.UtcNow.Date;
            var actualReturnDate = startDate.AddDays(7);
            var rental = new RentMoto { RentId = rentId, StartDate = startDate, PlanDays = planDays, DailyRate = 30, ActualReturnDate = actualReturnDate };
            var mockRentRepo = new Mock<IRentMotoRepository>();
            var mockDeliveryRepo = new Mock<IDeliveryPersonRepository>();
            mockRentRepo.Setup(r => r.GetRentalByIdAsync(rentId)).ReturnsAsync(rental);
            var service = new RentMotoService(mockRentRepo.Object, mockDeliveryRepo.Object);

            var value = await service.GetFinalRentalValueAsync(rentId);
            Assert.Equal(210m, value); // 7 dias * 30m = 210m (simplificado)
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SimulateReturnValueAsync_ShouldReturnSimulatedValue()
        {
            var rentId = "rent-123";
            var planDays = 7;
            var startDate = DateTime.UtcNow.Date;
            var returnDate = startDate.AddDays(10);
            var rental = new RentMoto { RentId = rentId, StartDate = startDate, PlanDays = planDays, DailyRate = 30 };
            var mockRentRepo = new Mock<IRentMotoRepository>();
            var mockDeliveryRepo = new Mock<IDeliveryPersonRepository>();
            mockRentRepo.Setup(r => r.GetRentalByIdAsync(rentId)).ReturnsAsync(rental);
            var service = new RentMotoService(mockRentRepo.Object, mockDeliveryRepo.Object);

            var value = await service.SimulateReturnValueAsync(rentId, returnDate);
            Assert.Equal(210m, value); // 7 dias * 30m = 210m (simplificado)
        }
    }
}
