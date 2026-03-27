using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services.Interfaces;
using UTSCITAS_API.Models;
using Microsoft.Extensions.Logging;

namespace UTSCITAS_API.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (usuario == null)
                return BadRequest("Usuario no puede ser nulo");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _usuarioService.CrearUsuario(usuario);
                return Ok();
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Validación inválida al crear usuario");
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando usuario");
                return StatusCode(500, "Ocurrió un error al crear el usuario");
            }
        }
    }
}
