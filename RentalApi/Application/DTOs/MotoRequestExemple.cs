using Swashbuckle.AspNetCore.Filters;

namespace RentalApi.Application.DTOs
{
    public class CreateMotoRequestExample : IExamplesProvider<CreateMotoRequest>
    {
        public CreateMotoRequest GetExamples()
        {
            return new CreateMotoRequest
            {
                Identifier = "moto123",
                Year = 2020,
                MotorcycleModel = "Mottu Sport",
                LicensePlate = "CDX-0101"
            };
        }
    }

    public class UpdateMotoPlacaRequestExample : IExamplesProvider<UpdateMotoPlacaRequest>
    {
        public UpdateMotoPlacaRequest GetExamples()
        {
            return new UpdateMotoPlacaRequest
            {
                LicensePlate = "AB-1234"
            };
        }
    }
}