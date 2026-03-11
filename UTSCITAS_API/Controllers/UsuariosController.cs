using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services.Interfaces;
using UTSCITAS_API.Models;

namespace UTSCITAS_API.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            await _usuarioService.CrearUsuario(usuario);
            return Ok();
        }
    }
}
