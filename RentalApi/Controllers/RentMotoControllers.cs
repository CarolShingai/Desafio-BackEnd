using Microsoft.AspNetCore.Mvc;
using RentalApi.Application.Services;
using RentalApi.Application.DTOs;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Controllers
{
    [ApiController]
    [Route("locacao")]
    [Tags("locação")]
    public class RentMotoControllers : ControllerBase
    {
        private readonly IRentMotoService _rentMotoService;

        public RentMotoControllers(IRentMotoService rentMotoService)
        {
            _rentMotoService = rentMotoService;
        }

        [HttpPost]
        public async Task<IActionResult> RentMotorcycle([FromBody] CreateRentalRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rentMotoService.CreateRentalAsync
                (request.DeliveryPersonId, request.MotoId, request.PlanDays);
            return CreatedAtRoute(null, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRentById(string id)
        {
            var result = await _rentMotoService.GetRentalByIdAsync(id);
            if (result == null)
                return NotFound(new RentalErrorResponse { Message = "Locação não encontrada" });
            return Ok(result);
        }

        [HttpPut("{id}/devolucao")]
        public async Task<IActionResult> UpdateRent(string id, [FromBody] InformReturnDateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _rentMotoService.SimulateReturnValueAsync(id, request.ActualReturnDate);
                return Ok(result);   
            }
            catch (Exception ex)
            {
                return NotFound(new RentalErrorResponse { Message = "Dados inválidos" + ex.Message });
            }
        }
    }
}