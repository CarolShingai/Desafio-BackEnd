using Swashbuckle.AspNetCore.Filters;

namespace RentalApi.Application.DTOs
{
    /// <summary>
    /// Provides an example for the CreateDeliveryPersonRequest DTO used in Swagger documentation.
    /// </summary>
    public class DeliveryPersonRequestExample : IExamplesProvider<CreateDeliveryPersonRequest>
    {
        /// <summary>
        /// Returns an example instance of CreateDeliveryPersonRequest for Swagger UI.
        /// </summary>
        /// <returns>Example CreateDeliveryPersonRequest object.</returns>
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