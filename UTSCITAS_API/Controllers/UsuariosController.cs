using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services.Interfaces;
using UTSCITAS_API.Models;

namespace UTSCITAS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] Usuario usuario)
        {
            await _usuarioService.CrearUsuario(usuario);
            return Ok();
        }
    }
}
