using Swashbuckle.AspNetCore.Filters;

namespace RentalApi.Application.DTOs
{
    /// <summary>
    /// Provides an example for the CreateMotoRequest DTO used in Swagger documentation.
    /// </summary>
    public class CreateMotoRequestExample : IExamplesProvider<CreateMotoRequest>
    {
        /// <summary>
        /// Returns an example instance of CreateMotoRequest for Swagger UI.
        /// </summary>
        /// <returns>Example CreateMotoRequest object.</returns>
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

    /// <summary>
    /// Provides an example for the UpdateMotoPlacaRequest DTO used in Swagger documentation.
    /// </summary>
    public class UpdateMotoPlacaRequestExample : IExamplesProvider<UpdateMotoPlacaRequest>
    {
        /// <summary>
        /// Returns an example instance of UpdateMotoPlacaRequest for Swagger UI.
        /// </summary>
        /// <returns>Example UpdateMotoPlacaRequest object.</returns>
        public UpdateMotoPlacaRequest GetExamples()
        {
            return new UpdateMotoPlacaRequest
            {
                LicensePlate = "AB-1234"
            };
        }
    }
}