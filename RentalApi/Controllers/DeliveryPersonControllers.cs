using Microsoft.AspNetCore.Mvc;
using RentalApi.Application.Services;
using RentalApi.Application.DTOs;
using RentalApi.Domain.Entities;

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
        public async Task<IActionResult> RegisterDeliveryPerson([FromBody] DeliveryPerson deliveryPers)
        {
            if (deliveryPers == null)
                return BadRequest("Invalid delivery person data.");

            var delivery = new DeliveryPerson
            {
                Name = deliveryPers.Name,
                Cnpj = deliveryPers.Cnpj,
                BirthDate = deliveryPers.BirthDate,
                Cnh = deliveryPers.Cnh,
                CnhType = deliveryPers.CnhType,
                CnhImage = deliveryPers.CnhImage
            };

            var result = await _deliveryPersonService.RegisterDeliveryPersonAsync(delivery);
            return CreatedAtRoute(null, result);
        }
        [HttpPost("{id}/cnh")]
        public async Task<IActionResult> UpdateCnhImage(Guid id, [FromBody] string base64Image)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
                return BadRequest("Invalid image data.");

            var result = await _deliveryPersonService.UpdateCnhImageAsync(id, base64Image);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}