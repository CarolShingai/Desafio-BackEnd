using Moq;
using RentalApi.Application.Services;
using RentalApi.Domain.Entities;
using RentalApi.Domain.Interfaces;
using Xunit;

namespace Tests.RentalApi.Tests
{
    public class MotoServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllMoto_ShouldReturnAllMotos()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var expectedMotos = new List<Moto>
            {
                new Moto { Identificador = "moto1", Modelo = "Honda CB", Placa = "ABC-1234", Ano = 2023 },
                new Moto { Identificador = "moto2", Modelo = "Yamaha MT", Placa = "XYZ-5678", Ano = 2024 }
            };
            mockRepository.Setup(r => r.FindMotoAllAsync()).ReturnsAsync(expectedMotos);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var result = await service.GetAllMoto();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Honda CB", result[0].Modelo);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAllMoto_EmptyList_ShouldReturnEmptyList()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            mockRepository.Setup(r => r.FindMotoAllAsync()).ReturnsAsync(new List<Moto>());
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var result = await service.GetAllMoto();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetMotoByIdentifierAsync_MotoExists_ShouldReturnMoto()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var expectedMoto = new Moto { Identificador = "moto1", Modelo = "Honda CB", Placa = "ABC-1234" };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(expectedMoto);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var result = await service.GetMotoByIdentifierAsync("moto1");

            Assert.NotNull(result);
            Assert.Equal("Honda CB", result.Modelo);
            Assert.Equal("ABC-1234", result.Placa);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetMotoByIdentifierAsync_MotoDoesNotExist_ShouldReturnNull()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("notfound")).ReturnsAsync((Moto?)null);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var result = await service.GetMotoByIdentifierAsync("notfound");

            Assert.Null(result);
        }

        // Removed duplicated and invalid code blocks

        [Fact]
        [Trait("Category", "Unit")]
        public async Task RegisterNewMotoAsync_DuplicateLicense_ShouldThrowException()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identificador = "moto1", Placa = "ABC-1234" };
            var newMoto = new Moto { Placa = "ABC-1234", Modelo = "Honda", Ano = 2023 };
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("ABC-1234")).ReturnsAsync(existingMoto);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => service.RegisterNewMotoAsync(newMoto));
            Assert.Equal("The motorcycle with the same license plate already exists.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_MotoExists_UniqueLicense_ShouldUpdateSuccessfully()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identificador = "moto1", Modelo = "Honda", Placa = "ABC-1234", Ano = 2023 };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("NEW-5678")).ReturnsAsync((Moto?)null);
            mockRepository.Setup(r => r.UpdateMotoLicenseAsync("moto1", "NEW-5678")).ReturnsAsync(true);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var result = await service.ChangeMotoLicenseAsync("moto1", "NEW-5678");

            Assert.True(result);
            mockRepository.Verify(r => r.UpdateMotoLicenseAsync("moto1", "NEW-5678"), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_MotoDoesNotExist_ShouldThrowException()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("notfound")).ReturnsAsync((Moto?)null);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => service.ChangeMotoLicenseAsync("notfound", "NEW-5678"));
            Assert.Equal("Motorcycle not found.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_LicenseAlreadyExists_ShouldThrowException()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identificador = "moto1", Modelo = "Honda", Placa = "ABC-1234" };
            var otherMoto = new Moto { Identificador = "moto2", Modelo = "Yamaha", Placa = "XYZ-5678" };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("XYZ-5678")).ReturnsAsync(otherMoto);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => service.ChangeMotoLicenseAsync("moto1", "XYZ-5678"));
            Assert.Equal("License plate already exists on another motorcycle.", exception.Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ChangeMotoLicenseAsync_SameMoto_SameLicense_ShouldAllow()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identificador = "moto1", Modelo = "Honda", Placa = "ABC-1234" };
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.FindByMotoLicenseAsync("ABC-1234")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.UpdateMotoLicenseAsync("moto1", "ABC-1234")).ReturnsAsync(true);
            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var result = await service.ChangeMotoLicenseAsync("moto1", "ABC-1234");

            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteRegisteredMotoAsync_MotoExists_ShouldRemoveSuccessfully()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            var existingMoto = new Moto { Identificador = "moto1", Modelo = "Honda", Placa = "ABC-1234" };

            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("moto1")).ReturnsAsync(existingMoto);
            mockRepository.Setup(r => r.RemoveMotoAsync("moto1")).ReturnsAsync(true);

            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var result = await service.DeleteRegisteredMotoAsync("moto1");

            Assert.True(result);
            mockRepository.Verify(r => r.RemoveMotoAsync("moto1"), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteRegisteredMotoAsync_MotoDoesNotExist_ShouldThrowException()
        {
            var mockRepository = new Mock<IMotoRepository>();
            var mockPublisher = new Mock<IMessagePublisher>();
            mockRepository.Setup(r => r.FindByMotoIdentifierAsync("notfound")).ReturnsAsync((Moto?)null);

            var service = new MotoService(mockRepository.Object, mockPublisher.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => service.DeleteRegisteredMotoAsync("notfound"));
            Assert.Equal("Motorcycle not found.", exception.Message);
        }
    }
}