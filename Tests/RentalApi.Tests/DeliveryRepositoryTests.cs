using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalApi.Domain.Entities;
using RentalApi.Infrastructure.Data;
using RentalApi.Infrastructure.Repositories;
using Xunit;

namespace RentalApi.Tests
{
    public class DeliveryRepositoryTests
    {
        private DeliveryRepository GetRepository()
        {
            var options = new DbContextOptionsBuilder<RentalDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new RentalDbContext(options);
            return new DeliveryRepository(context);
        }

        [Fact]
        public async Task AddDeliveryPersonAsync_ShouldAdd_WhenValid()
        {
            var repo = GetRepository();
            var person = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Name = "Teste",
                Cnpj = "12345678901234",
                BirthDate = "1990-01-01",
                Cnh = "12345678901",
                CnhType = "A",
                CnhImage = "base64string"
            };
            var result = await repo.AddDeliveryPersonAsync(person);
            Assert.NotNull(result);
            Assert.Equal(person.Cnpj, result.Cnpj);
        }

        [Fact]
        public async Task AddDeliveryPersonAsync_ShouldThrow_WhenCnpjExists()
        {
            var repo = GetRepository();
            var person1 = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Name = "Teste1",
                Cnpj = "12345678901234",
                BirthDate = "1990-01-01",
                Cnh = "12345678901",
                CnhType = "A",
                CnhImage = "base64string"
            };
            var person2 = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Name = "Teste2",
                Cnpj = "12345678901234",
                BirthDate = "1991-01-01",
                Cnh = "12345678902",
                CnhType = "B",
                CnhImage = "base64string"
            };
            await repo.AddDeliveryPersonAsync(person1);
            await Assert.ThrowsAsync<ArgumentException>(() => repo.AddDeliveryPersonAsync(person2));
        }

        [Fact]
        public async Task AddDeliveryPersonAsync_ShouldThrow_WhenCnhExists()
        {
            var repo = GetRepository();
            var person1 = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Name = "Teste1",
                Cnpj = "12345678901234",
                BirthDate = "1990-01-01",
                Cnh = "12345678901",
                CnhType = "A",
                CnhImage = "base64string"
            };
            var person2 = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Name = "Teste2",
                Cnpj = "12345678901235",
                BirthDate = "1991-01-01",
                Cnh = "12345678901",
                CnhType = "B",
                CnhImage = "base64string"
            };
            await repo.AddDeliveryPersonAsync(person1);
            await Assert.ThrowsAsync<ArgumentException>(() => repo.AddDeliveryPersonAsync(person2));
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("A + B")]
        public void IsValidCnhType_ShouldReturnTrue_ForValidTypes(string type)
        {
            var repo = GetRepository();
            Assert.True(repo.IsValidCnhType(type));
        }

        [Theory]
        [InlineData("")]
        [InlineData("C")]
        [InlineData("AB")]
        [InlineData("A+B")]
        public void IsValidCnhType_ShouldReturnFalse_ForInvalidTypes(string type)
        {
            var repo = GetRepository();
            Assert.False(repo.IsValidCnhType(type));
        }

        [Theory]
        [InlineData("12345678901234", true)]
        [InlineData("1234567890123", false)]
        [InlineData("123456789012345", false)]
        [InlineData("abc45678901234", false)]
        public async Task ValidateCnpjAsync_ShouldValidateFormat(string cnpj, bool expected)
        {
            var repo = GetRepository();
            var result = await repo.ValidateCnpjAsync(cnpj);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("12345678901", true)]
        [InlineData("1234567890", false)]
        [InlineData("123456789012", false)]
        [InlineData("abc45678901", false)]
        public async Task ValidateCnhAsync_ShouldValidateFormat(string cnh, bool expected)
        {
            var repo = GetRepository();
            var result = await repo.ValidateCnhAsync(cnh);
            Assert.Equal(expected, result);
        }
    }
}
