using Swashbuckle.AspNetCore.Filters;

namespace RentalApi.Application.DTOs
{
    public class DeliveryPersonRequestExample : IExamplesProvider<CreateDeliveryPersonRequest>
    {
        public CreateDeliveryPersonRequest GetExamples()
        {
            return new CreateDeliveryPersonRequest
            {
                Identifier = "entregador123",
                Name = "João da Silva",
                Cnpj = "12345678000195",
                BirthDate = new DateTime(1990, 1, 1),
                CnhNumber = "12345678900",
                CnhType = "A",
                CnhImage = "base64ImageString"
            };
        }
    }
}