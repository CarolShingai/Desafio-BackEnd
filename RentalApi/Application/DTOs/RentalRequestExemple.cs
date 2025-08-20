using Swashbuckle.AspNetCore.Filters;

namespace RentalApi.Application.DTOs
{
    /// <summary>
    /// Provides an example for the CreateRentalRequest DTO used in Swagger documentation.
    /// </summary>
    public class CreateRentalRequestExample : IExamplesProvider<CreateRentalRequest>
    {
        /// <summary>
        /// Returns an example instance of CreateRentalRequest for Swagger UI.
        /// </summary>
        /// <returns>Example CreateRentalRequest object.</returns>
        public CreateRentalRequest GetExamples()
        {
            return new CreateRentalRequest
            {
                DeliveryPersonId = "entregador123",
                MotoId = "moto123",
                PlanDays = 7
            };
        }
    }

    /// <summary>
    /// Provides an example for the InformReturnDateRequest DTO used in Swagger documentation.
    /// </summary>
    public class InformReturnDateRequestExample : IExamplesProvider<InformReturnDateRequest>
    {
        /// <summary>
        /// Returns an example instance of InformReturnDateRequest for Swagger UI.
        /// </summary>
        /// <returns>Example InformReturnDateRequest object.</returns>
        public InformReturnDateRequest GetExamples()
        {
            return new InformReturnDateRequest
            {
                ActualReturnDate = DateTime.UtcNow.Date.AddDays(8)
            };
        }
    }
}