using Microsoft.AspNetCore.Mvc;
using RentalApi.Application.Services;
using RentalApi.Application.DTOs;

namespace RentalApi.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão de entregadores
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
        /// Cadastra um novo entregador no sistema
        /// </summary>
        /// <param name="dto">Dados do entregador a ser cadastrado</param>
        /// <returns>Dados do entregador cadastrado</returns>
        /// <response code="201">Entregador cadastrado com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="409">CNPJ ou CNH já cadastrados</response>
        /// <example>
        /// {
        ///   "identifier": "ENT001",
        ///   "name": "João Silva",
        ///   "cnpj": "12.345.678/0001-90",
        ///   "birthDate": "1990-05-15",
        ///   "cnhNumber": "12345678901",
        ///   "cnhType": "A",
        ///   "cnhImagePath": "/path/to/image.jpg"
        /// }
        /// </example>
        [HttpPost]
        [ProducesResponseType(typeof(DeliveryPersonResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> RegisterDeliveryPerson([FromBody] CreateDeliveryPersonRequest dto)
        {
            if (dto == null)
                return BadRequest("Invalid delivery person data.");

            var result = await _deliveryPersonService.RegisterDeliveryPersonAsync(dto);
            return CreatedAtRoute(null, result);
        }

        /// <summary>
        /// Atualiza a imagem da CNH de um entregador
        /// </summary>
        /// <param name="id">Identificador único do entregador</param>
        /// <param name="request">Nova imagem da CNH em base64</param>
        /// <returns>Confirmação da atualização</returns>
        /// <response code="204">Imagem atualizada com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="404">Entregador não encontrado</response>
        [HttpPost("{id}/cnh")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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