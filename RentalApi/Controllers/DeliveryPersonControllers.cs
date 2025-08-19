using Microsoft.AspNetCore.Mvc;
using RentalApi.Application.Services;
using RentalApi.Application.DTOs;

namespace RentalApi.Controllers
{
    [ApiController]
    [Route("Entregadores")]
    [Tags("entregadores")]
    public class DeliveryPersonController : ControllerBase
    {
        private readonly IDeliveryPersonService _deliveryPersonService;

        public DeliveryPersonController(IDeliveryPersonService deliveryPersonService)
        {
            _deliveryPersonService = deliveryPersonService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDeliveryPerson([FromBody] CreateDeliveryPersonRequest dto)
        {
            if (dto == null)
                return BadRequest("Invalid delivery person data.");

            var result = await _deliveryPersonService.RegisterDeliveryPersonAsync(dto);
            return CreatedAtRoute(null, result);
        }

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