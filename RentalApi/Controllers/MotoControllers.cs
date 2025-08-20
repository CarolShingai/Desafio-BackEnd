using Microsoft.AspNetCore.Mvc;
using RentalApi.Application.Services;
using RentalApi.Application.DTOs;
using RentalApi.Domain.Entities;

namespace RentalApi.Controllers
{
    /// <summary>
    /// Controller responsible for managing motorcycles.
    /// </summary>
    [ApiController]
    [Route("moto")]
    [Tags("motos")]
    public class MotoControllers : ControllerBase
    {
        private readonly MotoService _motoService;

    /// <summary>
    /// Constructor for the motorcycle controller.
    /// </summary>
    /// <param name="motoService">Injected motorcycle service.</param>
        public MotoControllers(MotoService motoService)
        {
            _motoService = motoService;
        }

    /// <summary>
    /// Creates a new motorcycle in the system.
    /// </summary>
    /// <param name="request">Motorcycle data to be created.</param>
    /// <returns>Data of the created motorcycle.</returns>
    /// <response code="400">Invalid input data</response>
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
    /// Returns all motorcycles registered in the system.
    /// </summary>
    /// <returns>Complete list of registered motorcycles.</returns>
    /// <response code="400">Invalid input data</response>
        [HttpGet]
        [ProducesResponseType(400)]
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
    /// Retrieves a motorcycle by its identifier.
    /// </summary>
    /// <param name="id">Motorcycle identifier.</param>
    /// <returns>Motorcycle data or error.</returns>
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
    /// Updates the license plate of a motorcycle.
    /// </summary>
    /// <param name="id">Motorcycle identifier.</param>
    /// <param name="request">New license plate.</param>
    /// <returns>Operation status.</returns>
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
    /// Removes a motorcycle by its identifier.
    /// </summary>
    /// <param name="id">Motorcycle identifier.</param>
    /// <returns>Operation status.</returns>
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
