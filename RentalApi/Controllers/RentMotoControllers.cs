using Microsoft.AspNetCore.Mvc;
using RentalApi.Application.Services;
using RentalApi.Application.DTOs;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Controllers
{
    /// <summary>
    /// Controller responsible for managing motorcycle rentals.
    /// </summary>
    [ApiController]
    [Route("locacao")]
    [Tags("locação")]
    [Produces("application/json")]
    public class RentMotoControllers : ControllerBase
    {
        private readonly IRentMotoService _rentMotoService;

        public RentMotoControllers(IRentMotoService rentMotoService)
        {
            _rentMotoService = rentMotoService;
        }

    /// <summary>
    /// Creates a new motorcycle rental.
    /// </summary>
    /// <param name="request">Rental data to be created</param>
    /// <returns>Created rental data</returns>
    /// <response code="400">Invalid input data</response>
        [HttpPost]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RentMotorcycle([FromBody] CreateRentalRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rentMotoService.CreateRentalAsync
                (request.DeliveryPersonId, request.MotoId, request.PlanDays);
            return CreatedAtRoute(null, result);
        }

    /// <summary>
    /// Retrieves a specific rental by ID.
    /// </summary>
    /// <param name="id">Unique rental identifier</param>
    /// <returns>Rental data if found</returns>
    /// <response code="400">Invalid input data</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRentById(string id)
        {
            var result = await _rentMotoService.GetRentalByIdAsync(id);
            if (result == null)
                return NotFound(new RentalErrorResponse { Message = "Locação não encontrada" });
            return Ok(result);
        }

    /// <summary>
    /// Sets the return date for the motorcycle and calculates the final value.
    /// </summary>
    /// <param name="id">Unique rental identifier</param>
    /// <param name="request">Return date data</param>
    /// <returns>Total rental value with penalties/discounts applied</returns>
    /// <response code="400">Invalid input data</response>
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