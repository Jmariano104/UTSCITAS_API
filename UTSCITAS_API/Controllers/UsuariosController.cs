using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services;
using UTSCITAS_API.DTOs;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;

namespace UTSCITAS_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _service;

    public UsuariosController(UsuarioService service) => _service = service;

    // GET api/usuarios
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var data = await _service.ListarAsync();
        return Ok(data);
    }

    // GET api/usuarios/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var usuario = await _service.BuscarPorIdAsync(id);
        if (usuario is null) return NotFound(new { mensaje = "Usuario no encontrado." });
        return Ok(usuario);
    }

    // POST api/usuarios/crear
    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] UsuarioDto dto)
    {
        try
        {
            int v = await _service.InsertarAsync(dto);
            return Ok(v);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                mensaje = ex.Message,
                detalle = ex.InnerException?.Message
            });
        }
    }

    // PUT api/usuarios/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] UsuarioDto dto)
    {
        await _service.ActualizarAsync(id, dto);
        return NoContent();
    }

    // DELETE api/usuarios/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _service.EliminarAsync(id);
        return NoContent();
    }

    // POST api/usuarios/login
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        var usuario = await _service.BuscarPorCorreoAsync(dto.Email);
        if (usuario is null) return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });
        // Verificar contraseña
        var isValid = await _service.CompararLoginAsync(usuario.Password, dto.Password);
        if (!isValid) return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });

        return Ok(new { mensaje = "Login exitoso.", usuario.IdUsuario, usuario.Nombre, usuario.Correo });
    }
}
