using Swashbuckle.AspNetCore.Filters;

namespace RentalApi.Application.DTOs
{
    public class CreateRentalRequestExample : IExamplesProvider<CreateRentalRequest>
    {
        public CreateRentalRequest GetExamples()
        {
            return new CreateRentalRequest
            {
                DeliveryPersonId = "entregador123",
                MotoId = "1",
                PlanDays = 7
            };
        }
    }

    public class InformReturnDateRequestExample : IExamplesProvider<InformReturnDateRequest>
    {
        public InformReturnDateRequest GetExamples()
        {
            return new InformReturnDateRequest
            {
                ActualReturnDate = DateTime.UtcNow.Date.AddDays(8)
            };
        }
    }
}