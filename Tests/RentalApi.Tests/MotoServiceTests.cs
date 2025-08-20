using Moq;
using RentalApi.Application.Services;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using Xunit;

namespace Tests.RentalApi.Tests
{
    /// <summary>
    /// Unit tests for MotoService class validating business logic and validations
    /// </summary>
    public class MotoServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllMoto_ShouldReturnAllMotos_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var expectedMotos = new List<Moto>
            {
                new Moto { Identifier = "moto1", MotorcycleModel = "Honda CB", LicensePlate = "ABC-1234", Year = 2023 },
                new Moto { Identifier = "moto2", MotorcycleModel = "Yamaha MT", LicensePlate = "XYZ-5678", Year = 2024 }
            };
            mockRepository.Setup(r => r.FindMotoAllAsync()).ReturnsAsync(expectedMotos);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.GetAllMoto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Honda CB", result[0].MotorcycleModel);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllMoto_EmptyList_ShouldReturnEmptyList_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            mockRepository.Setup(r => r.FindMotoAllAsync()).ReturnsAsync(new List<Moto>());
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.GetAllMoto();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetMotoByIdentifierAsync_MotoExists_ShouldReturnMoto_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var expectedMoto = new Moto { Identifier = "moto1", MotorcycleModel = "Honda CB", LicensePlate = "ABC-1234" };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(expectedMoto);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.GetMotoByIdentifierAsync("moto1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Honda CB", result.MotorcycleModel);
            Assert.Equal("ABC-1234", result.LicensePlate);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetMotoByIdentifierAsync_MotoDoesNotExist_ShouldReturnNull_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("notfound")).ReturnsAsync((Moto?)null);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.GetMotoByIdentifierAsync("notfound");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RegisterNewMotoAsync_ValidData_ShouldRegisterSuccessfully_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var newMoto = new Moto 
            { 
                Identifier = "moto1", 
                MotorcycleModel = "Honda CB", 
                LicensePlate = "ABC-1234", 
                Year = 2023 
            };
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("ABC-1234")).ReturnsAsync((Moto?)null);
            mockRepository.Setup(r => r.AddMotoAsync(It.IsAny<Moto>())).ReturnsAsync(newMoto);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.RegisterNewMotoAsync(newMoto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ABC-1234", result.LicensePlate);
            mockPublisher.Verify(p => p.PublishAsync(It.IsAny<Moto>(), It.Is<string>(s => s == "motoQueue"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RegisterNewMotoAsync_InvalidYear_ShouldThrowException_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var newMoto = new Moto 
            { 
                LicensePlate = "ABC-1234", 
                Year = 1999 // Invalid year
            };
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.RegisterNewMotoAsync(newMoto));
            Assert.Equal("Invalid year. The motorcycle year must be between 2000 and the current year.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RegisterNewMotoAsync_InvalidLicensePlateFormat_ShouldThrowException_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var newMoto = new Moto 
            { 
                LicensePlate = "INVALID", // Invalid format
                Year = 2023 
            };
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.RegisterNewMotoAsync(newMoto));
            Assert.Equal("Invalid license plate format. Use the format XXX-1111.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RegisterNewMotoAsync_LicensePlateSanitization_ShouldTrimAndUppercase_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var newMoto = new Moto 
            { 
                LicensePlate = " abc-1234 ", // Lowercase with spaces
                Year = 2023 
            };
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("ABC-1234")).ReturnsAsync((Moto?)null);
            mockRepository.Setup(r => r.AddMotoAsync(It.IsAny<Moto>())).ReturnsAsync(newMoto);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.RegisterNewMotoAsync(newMoto);

            // Assert
            Assert.Equal("ABC-1234", newMoto.LicensePlate); // Should be sanitized
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RegisterNewMotoAsync_DuplicateLicense_ShouldThrowException_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identifier = "moto1", LicensePlate = "ABC-1234" };
            var newMoto = new Moto { LicensePlate = "ABC-1234", MotorcycleModel = "Honda", Year = 2023 };
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("ABC-1234")).ReturnsAsync(existingMoto);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.RegisterNewMotoAsync(newMoto));
            Assert.Equal("The motorcycle with the same license plate already exists.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_ValidData_ShouldUpdateSuccessfully_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identifier = "moto1", MotorcycleModel = "Honda", LicensePlate = "ABC-1234", Year = 2023 };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("NEW-5678")).ReturnsAsync((Moto?)null);
            mockRepository.Setup(r => r.UpdateMotoLicenseAsync("moto1", "NEW-5678")).ReturnsAsync(true);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.ChangeMotoLicenseAsync("moto1", "NEW-5678");

            // Assert
            Assert.True(result);
            mockRepository.Verify(r => r.UpdateMotoLicenseAsync("moto1", "NEW-5678"), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_InvalidLicensePlateFormat_ShouldThrowException_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.ChangeMotoLicenseAsync("moto1", "INVALID"));
            Assert.Equal("Invalid license plate format. Use the format XXX-1111.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_LicensePlateSanitization_ShouldTrimAndUppercase_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identifier = "moto1", LicensePlate = "ABC-1234" };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("NEW-5678")).ReturnsAsync((Moto?)null);
            mockRepository.Setup(r => r.UpdateMotoLicenseAsync("moto1", "NEW-5678")).ReturnsAsync(true);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.ChangeMotoLicenseAsync("moto1", " new-5678 "); // Lowercase with spaces

            // Assert
            Assert.True(result);
            // Verify the sanitized version was used
            mockRepository.Verify(r => r.UpdateMotoLicenseAsync("moto1", "NEW-5678"), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_MotoDoesNotExist_ShouldThrowException_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("notfound")).ReturnsAsync((Moto?)null);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.ChangeMotoLicenseAsync("notfound", "NEW-5678"));
            Assert.Equal("Motorcycle not found.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_LicenseAlreadyExists_ShouldThrowException_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identifier = "moto1", MotorcycleModel = "Honda", LicensePlate = "ABC-1234" };
            var otherMoto = new Moto { Identifier = "moto2", MotorcycleModel = "Yamaha", LicensePlate = "XYZ-5678" };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("XYZ-5678")).ReturnsAsync(otherMoto);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.ChangeMotoLicenseAsync("moto1", "XYZ-5678"));
            Assert.Equal("License plate already exists on another motorcycle.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_SameMotoSameLicense_ShouldAllow_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identifier = "moto1", MotorcycleModel = "Honda", LicensePlate = "ABC-1234" };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("ABC-1234")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.UpdateMotoLicenseAsync("moto1", "ABC-1234")).ReturnsAsync(true);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.ChangeMotoLicenseAsync("moto1", "ABC-1234");

            // Assert
            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteRegisteredMotoAsync_MotoExists_ShouldRemoveSuccessfully_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identifier = "moto1", MotorcycleModel = "Honda", LicensePlate = "ABC-1234" };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.RemoveMotoAsync("moto1")).ReturnsAsync(true);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act
            var result = await service.DeleteRegisteredMotoAsync("moto1");

            // Assert
            Assert.True(result);
            mockRepository.Verify(r => r.RemoveMotoAsync("moto1"), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteRegisteredMotoAsync_MotoDoesNotExist_ShouldThrowException_UnitTest()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("notfound")).ReturnsAsync((Moto?)null);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.DeleteRegisteredMotoAsync("notfound"));
            Assert.Equal("Motorcycle not found.", exception.Message);
        }
    }
}