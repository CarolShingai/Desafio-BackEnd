using Microsoft.AspNetCore.Mvc;
using RentalApi.Application.Services;
using RentalApi.Application.DTOs;
using RentalApi.Domain.Entities;

namespace RentalApi.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de motos.
    /// </summary>
    [ApiController]
    [Route("moto")]
    [Tags("motos")]
    public class MotoControllers : ControllerBase
    {
        private readonly MotoService _motoService;

        /// <summary>
        /// Construtor do controller de motos.
        /// </summary>
        /// <param name="motoService">Serviço de motos injetado.</param>
        public MotoControllers(MotoService motoService)
        {
            _motoService = motoService;
        }

        /// <summary>
        /// Cria uma nova moto no sistema.
        /// </summary>
        /// <param name="request">Dados da moto a ser criada.</param>
        /// <returns>Dados da moto criada com sucesso.</returns>
        /// <response code="201">Moto criada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <example>
        /// POST /moto
        /// {
        ///   "identifier": "MOTO001",
        ///   "year": 2024,
        ///   "motorcycleModel": "Honda CG 160",
        ///   "licensePlate": "ABC-1234"
        /// }
        /// </example>
        [HttpPost]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMoto([FromBody] CreateMotoRequest request)
        {
            var moto = new Moto
            {
                Identifier = request.Identifier,
                Year = request.Year,
                MotorcycleModel = request.MotorcycleModel,
                LicensePlate = request.LicensePlate
            };
            var createdMoto = await _motoService.RegisterNewMotoAsync(moto);
            var response = new MotoResponse
            {
                Identifier = createdMoto.Identifier,
                Year = createdMoto.Year,
                MotorcycleModel = createdMoto.MotorcycleModel,
                LicensePlate = createdMoto.LicensePlate
            };
            return CreatedAtAction(nameof(GetMotoById), new { id = response.Identifier }, response);
        }

        /// <summary>
        /// Retorna todas as motos cadastradas no sistema.
        /// </summary>
        /// <returns>Lista completa de motos cadastradas.</returns>
        /// <response code="200">Lista de motos retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        public async Task<IActionResult> GetAllMotos()
        {
            var motos = await _motoService.GetAllMoto();
            var response = motos.Select(m => new MotoResponse
            {
                Identifier = m.Identifier,
                Year = m.Year,
                MotorcycleModel = m.MotorcycleModel,
                LicensePlate = m.LicensePlate
            });
            return Ok(response);
        }

        /// <summary>
        /// Busca uma moto pelo identificador.
        /// </summary>
        /// <param name="id">Identificador da moto.</param>
        /// <returns>Dados da moto encontrada ou erro.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMotoById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "Request mal Formada" });

            var moto = await _motoService.GetMotoByIdentifierAsync(id);
            if (moto == null)
                return NotFound(new { message = "Moto não encontrada" });

            var response = new MotoResponse
            {
                Identifier = moto.Identifier,
                Year = moto.Year,
                MotorcycleModel = moto.MotorcycleModel,
                LicensePlate = moto.LicensePlate
            };
            return Ok(response);
        }

        /// <summary>
        /// Atualiza a placa de uma moto.
        /// </summary>
        /// <param name="id">Identificador da moto.</param>
        /// <param name="request">Nova placa.</param>
        /// <returns>Status da operação.</returns>
        [HttpPut("{id}/placa")]
        public async Task<IActionResult> UpdateMotoPlaca(string id, [FromBody] UpdateMotoPlacaRequest request)
        {
            try
            {
                var success = await _motoService.ChangeMotoLicenseAsync(id, request.LicensePlate);
                if (success)
                {
                    return Ok(new { message = "Placa modificada com sucesso" });
                }
                return BadRequest(new { message = "Erro ao atualizar a placa" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove uma moto pelo identificador.
        /// </summary>
        /// <param name="id">Identificador da moto.</param>
        /// <returns>Status da operação.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(string id)
        {
            try
            {
                await _motoService.DeleteRegisteredMotoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
