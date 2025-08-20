using Microsoft.AspNetCore.Mvc;
using RentalApi.Application.Services;
using RentalApi.Application.DTOs;
using RentalApi.Domain.Interfaces;

namespace RentalApi.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão de locações de motos
    /// </summary>
    [ApiController]
    [Route("locacao")]
    [Tags("Locação")]
    [Produces("application/json")]
    public class RentMotoControllers : ControllerBase
    {
        private readonly IRentMotoService _rentMotoService;

        public RentMotoControllers(IRentMotoService rentMotoService)
        {
            _rentMotoService = rentMotoService;
        }

        /// <summary>
        /// Cria uma nova locação de moto
        /// </summary>
        /// <param name="request">Dados da locação a ser criada</param>
        /// <returns>Dados da locação criada</returns>
        /// <response code="201">Locação criada com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="404">Moto ou entregador não encontrado</response>
        /// <response code="409">Moto não disponível para locação</response>
        [HttpPost]
        [ProducesResponseType(typeof(RentalResponse), 201)]
        [ProducesResponseType(typeof(RentalErrorResponse), 400)]
        [ProducesResponseType(typeof(RentalErrorResponse), 404)]
        [ProducesResponseType(typeof(RentalErrorResponse), 409)]
        public async Task<IActionResult> RentMotorcycle([FromBody] CreateRentalRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _rentMotoService.CreateRentalAsync
                (request.DeliveryPersonId, request.MotoId, request.PlanDays);
            return CreatedAtRoute(null, result);
        }

        /// <summary>
        /// Consulta uma locação específica por ID
        /// </summary>
        /// <param name="id">Identificador único da locação</param>
        /// <returns>Dados da locação encontrada</returns>
        /// <response code="200">Locação encontrada com sucesso</response>
        /// <response code="404">Locação não encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RentalResponse), 200)]
        [ProducesResponseType(typeof(RentalErrorResponse), 404)]
        public async Task<IActionResult> GetRentById(string id)
        {
            var result = await _rentMotoService.GetRentalByIdAsync(id);
            if (result == null)
                return NotFound(new RentalErrorResponse { Message = "Locação não encontrada" });
            return Ok(result);
        }

        /// <summary>
        /// Informa a data de devolução da moto e calcula o valor final
        /// </summary>
        /// <param name="id">Identificador único da locação</param>
        /// <param name="request">Data de retorno da moto</param>
        /// <returns>Valor total da locação com multas/descontos aplicados</returns>
        /// <response code="200">Data de devolução registrada com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="404">Locação não encontrada</response>
        [HttpPut("{id}/devolucao")]
        [ProducesResponseType(typeof(RentalValueResponseDto), 200)]
        [ProducesResponseType(typeof(RentalErrorResponse), 400)]
        [ProducesResponseType(typeof(RentalErrorResponse), 404)]
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