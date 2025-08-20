using Microsoft.AspNetCore.Mvc;
using RentalApi.Application.Services;
using RentalApi.Application.DTOs;

namespace RentalApi.Controllers
{
    /// <summary>
    /// Controller responsible for managing delivery persons.
    /// </summary>
    [ApiController]
    [Route("entregadores")]
    [Tags("Entregadores")]
    [Produces("application/json")]
    public class DeliveryPersonController : ControllerBase
    {
        private readonly IDeliveryPersonService _deliveryPersonService;

        public DeliveryPersonController(IDeliveryPersonService deliveryPersonService)
        {
            _deliveryPersonService = deliveryPersonService;
        }

    /// <summary>
    /// Registers a new delivery person in the system.
    /// </summary>
    /// <param name="dto">Delivery person data to be registered</param>
    /// <returns>Registered delivery person data</returns>
    /// <response code="400">Invalid input data</response>
    /// <example>
    /// {
    ///   "identifier": "entregador123",
    ///   "name": "João Silva",
    ///   "cnpj": "12.345.678/0001-90",
    ///   "birthDate": "1990-05-15",
    ///   "cnhNumber": "12345678901",
    ///   "cnhType": "A",
    ///   "cnhImagePath": "/path/to/image.jpg"
    /// }
    /// </example>
        [HttpPost]
        public async Task<IActionResult> RegisterDeliveryPerson([FromBody] CreateDeliveryPersonRequest dto)
        {
            if (dto == null)
                return BadRequest("Invalid delivery person data.");

            var result = await _deliveryPersonService.RegisterDeliveryPersonAsync(dto);
            return CreatedAtRoute(null, result);
        }

    /// <summary>
    /// Updates the CNH (driver's license) image of a delivery person.
    /// </summary>
    /// <param name="id">Unique identifier of the delivery person</param>
    /// <param name="request">New CNH image in base64</param>
    /// <returns>Confirmation of the update</returns>
    /// <response code="400">Invalid input data</response>
        [HttpPost("{id}/cnh")]
        public async Task<IActionResult> UpdateCnhImage(string id, [FromBody] UpdateImageRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UpdateCnhImageImage))
                return BadRequest("Invalid image data.");

            var result = await _deliveryPersonService.UpdateCnhImageAsync(id, request.UpdateCnhImageImage);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}