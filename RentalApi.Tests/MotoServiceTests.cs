using Moq;
using RentalApi.Application.Services;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using Xunit;

namespace RentalApi.Tests
{
    public class MotoServiceTests
    {
        [Fact]
        public async Task GetAllMoto_MustReturnAllMoto()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var expectedMotos = new List<Moto>
            {
                new Moto { Id = 1, Modelo = "Honda CB", Placa = "ABC-1234", Ano = 2023 },
                new Moto { Id = 2, Modelo = "Yamaha MT", Placa = "XYZ-5678", Ano = 2024 },
                new Moto { Id = 3, Modelo = "Yamaha MT", Placa = "XYV-1678", Ano = 2020 },
                new Moto { Id = 4, Modelo = "Honda MT", Placa = "TRG-1678", Ano = 2021 }
            };
            mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedMotos);
            var service = new MotoService(mockRepository.Object);

            // Act
            var result = await service.GetAllMoto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Equal("Honda CB", result.First().Modelo);
        }

        [Fact]
        public async Task GetAllMoto_EmptyList_MustReturnEmptyList()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var expectedMotos = new List<Moto>();
            mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedMotos);
            var service = new MotoService(mockRepository.Object);

            // Act
            var result = await service.GetAllMoto();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public async Task GetMotoByIdAsync_MotoExiste_DeveRetornarMoto()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var expectedMoto = new Moto { Id = 1, Modelo = "Honda CB", Placa = "ABC-1234" };
            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedMoto);
            var service = new MotoService(mockRepository.Object);

            // Act
            var result = await service.GetMotoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Honda CB", result.Modelo);
            Assert.Equal("ABC-1234", result.Placa);
        }

        [Fact]
        public async Task GetMotoByIdAsync_MotoNaoExiste_DeveRetornarNull()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Moto?)null);
            var service = new MotoService(mockRepository.Object);

            // Act
            var result = await service.GetMotoByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterNewMotoAsync_PlacaDuplicada_DeveLancarExcecao()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var existingMoto = new Moto { Id = 1, Placa = "ABC-1234" };
            var newMoto = new Moto { Placa = "ABC-1234", Modelo = "Honda", Ano = 2023 };
            mockRepository.Setup(r => r.GetByLicenseAsync("ABC-1234")).ReturnsAsync(existingMoto);
            var service = new MotoService(mockRepository.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.RegisterNewMotoAsync(newMoto));
            Assert.Equal("The motorcycle with the same license plate already exists.", exception.Message);
        }

        [Fact]
        public async Task ChangeMotoLicenseAsync_MotoExiste_PlacaUnica_DeveAtualizarComSucesso()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var existingMoto = new Moto { Id = 1, Modelo = "Honda", Placa = "ABC-1234", Ano = 2023 };
            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.GetByLicenseAsync("NEW-5678")).ReturnsAsync((Moto?)null);
            mockRepository.Setup(r => r.UpdateMotoLicenseAsync(1, "NEW-5678")).ReturnsAsync(true);
            var service = new MotoService(mockRepository.Object);

            // Act
            var result = await service.ChangeMotoLicenseAsync(1, "NEW-5678");

            // Assert
            Assert.True(result);
            mockRepository.Verify(r => r.UpdateMotoLicenseAsync(1, "NEW-5678"), Times.Once);
        }

        [Fact]
        public async Task ChangeMotoLicenseAsync_MotoNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Moto?)null);
            var service = new MotoService(mockRepository.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.ChangeMotoLicenseAsync(999, "NEW-5678"));
            Assert.Equal("Motorcycle not found.", exception.Message);
        }

        [Fact]
        public async Task ChangeMotoLicenseAsync_PlacaJaExiste_DeveLancarExcecao()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var existingMoto = new Moto { Id = 1, Modelo = "Honda", Placa = "ABC-1234" };
            var otherMoto = new Moto { Id = 2, Modelo = "Yamaha", Placa = "XYZ-5678" };
            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.GetByLicenseAsync("XYZ-5678")).ReturnsAsync(otherMoto);
            var service = new MotoService(mockRepository.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.ChangeMotoLicenseAsync(1, "XYZ-5678"));
            Assert.Equal("License plate already exists on another motorcycle.", exception.Message);
        }

        [Fact]
        public async Task ChangeMotoLicenseAsync_MesmaMoto_MesmaPlaca_DevePermitir()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var existingMoto = new Moto { Id = 1, Modelo = "Honda", Placa = "ABC-1234" };
            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.GetByLicenseAsync("ABC-1234")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.UpdateMotoLicenseAsync(1, "ABC-1234")).ReturnsAsync(true);
            var service = new MotoService(mockRepository.Object);

            // Act
            var result = await service.ChangeMotoLicenseAsync(1, "ABC-1234");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteRegisteredMotoAsync_MotoExiste_DeveRemoverComSucesso()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            var existingMoto = new Moto { Id = 1, Modelo = "Honda", Placa = "ABC-1234" };

            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.RemoveMotoAsync(1)).ReturnsAsync(true);

            var service = new MotoService(mockRepository.Object);

            // Act
            var result = await service.DeleteRegisteredMotoAsync(1);

            // Assert
            Assert.True(result);
            mockRepository.Verify(r => r.RemoveMotoAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteRegisteredMotoAsync_MotoNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            var mockRepository = new Mock<IMotoRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Moto?)null);

            var service = new MotoService(mockRepository.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.DeleteRegisteredMotoAsync(999));
            Assert.Equal("Motorcycle not found.", exception.Message);
        }
    }
}