using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Models;
using UTSCITAS_API.Services.Interfaces;


namespace UTSCITAS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly ICitaService _citaService;

        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] Cita cita)
        {
            bool creada = await _citaService.CrearCita(cita);

            if (!creada)
                return BadRequest("No se puede crear cita en día festivo");

            return Ok();
        }
    }
}
