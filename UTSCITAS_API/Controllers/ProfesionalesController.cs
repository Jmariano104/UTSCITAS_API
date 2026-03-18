using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services.Interfaces;

namespace UTSCITAS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfesionalesController : ControllerBase
    {
        private readonly IProfesionalService _profService;

        public ProfesionalesController(IProfesionalService profService)
        {
            _profService = profService;
        }

        [HttpGet("obtener")]
        public async Task<IActionResult> Obtener()
        {
            var lista = await _profService.ObtenerProfesionales();
            return Ok(lista);
        }
    }
}
