using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Models;
using UTSCITAS_API.Services.Interfaces;


namespace UTSCitas_API.Controllers
{
    public class CitasController : Controller
    {
        private readonly ICitaService _citaService;

        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Cita cita)
        {
            bool creada = await _citaService.CrearCita(cita);

            if (!creada)
                return BadRequest("No se puede crear cita en día festivo");

            return Ok();
        }
    }
}
