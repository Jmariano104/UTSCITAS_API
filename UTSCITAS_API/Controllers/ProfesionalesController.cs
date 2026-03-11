using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services.Interfaces;

namespace UTSCitas_API.Controllers
{
    public class ProfesionalesController : Controller
    {
        private readonly IProfesionalService _profService;

        public ProfesionalesController(IProfesionalService profService)
        {
            _profService = profService;
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var lista = await _profService.ObtenerProfesionales();
            return Ok(lista);
        }
    }
}
