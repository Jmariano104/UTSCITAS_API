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
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(UsuarioService service, ILogger<UsuariosController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // POST api/usuarios/crear
    [HttpPost("crear")]
    public async Task<IActionResult> Crear([FromBody] UsuarioDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.InsertarAsync(dto);
            return Ok(new { mensaje = "Usuario creado exitosamente", idUsuario = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear usuario: {ex.Message}");
            return StatusCode(500, new { mensaje = "Error al crear el usuario" });
        }
    }

    // GET api/usuarios
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        try
        {
            var usuarios = await _service.ListarAsync();
            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al listar usuarios: {ex.Message}");
            return StatusCode(500, new { mensaje = "Error al listar usuarios" });
        }
    }

    // GET api/usuarios/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            var usuario = await _service.BuscarPorIdAsync(id);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al buscar usuario: {ex.Message}");
            return StatusCode(500, new { mensaje = "Error al buscar usuario" });
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
