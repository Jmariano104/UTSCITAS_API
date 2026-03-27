using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Models;
using UTSCITAS_API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace UTSCitas_API.Controllers
{
    public class CitasController : Controller
    {
        private readonly ICitaService _citaService;
        private readonly ILogger<CitasController> _logger;

        public CitasController(ICitaService citaService, ILogger<CitasController> logger)
        {
            _citaService = citaService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Cita cita)
        {
            if (cita == null)
                return BadRequest("Cita no puede ser nula");

            try
            {
                bool creada = await _citaService.CrearCita(cita);

                if (!creada)
                    return BadRequest("No se puede crear cita en día festivo");

                return Ok();
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Validación inválida al crear cita");
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando cita");
                return StatusCode(500, "Ocurrió un error al crear la cita");
            }
        }
    }
}
