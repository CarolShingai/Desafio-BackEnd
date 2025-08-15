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
    [Route("api/[controller]")]
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
        /// Cria uma nova moto.
        /// </summary>
        /// <param name="request">Dados da moto a ser criada.</param>
        /// <returns>Dados da moto criada.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMoto([FromBody] CreateMotoRequest request)
        {
            var moto = new Moto
            {
                Identificador = request.Identificador,
                Ano = request.Ano,
                Modelo = request.Modelo,
                Placa = request.Placa
            };
            var createdMoto = await _motoService.RegisterNewMotoAsync(moto);
            var response = new MotoResponse
            {
                Identificador = createdMoto.Identificador,
                Ano = createdMoto.Ano,
                Modelo = createdMoto.Modelo,
                Placa = createdMoto.Placa
            };
            return CreatedAtAction(nameof(GetMotoById), new { id = response.Identificador }, response);
        }

        /// <summary>
        /// Retorna todas as motos cadastradas.
        /// </summary>
        /// <returns>Lista de motos.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllMotos()
        {
            var motos = await _motoService.GetAllMoto();
            var response = motos.Select(m => new MotoResponse
            {
                Identificador = m.Identificador,
                Ano = m.Ano,
                Modelo = m.Modelo,
                Placa = m.Placa
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
                Identificador = moto.Identificador,
                Ano = moto.Ano,
                Modelo = moto.Modelo,
                Placa = moto.Placa
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
                var success = await _motoService.ChangeMotoLicenseAsync(id, request.Placa);
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