using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services;
using UTSCITAS_API.DTOs;

namespace UTSCITAS_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerfilController : ControllerBase
{
    private readonly UsuarioService     _usuarioService;
    private readonly ProfesionalService _profesionalService;
    private readonly ILogger<PerfilController> _logger;

    public PerfilController(
        UsuarioService usuarioService,
        ProfesionalService profesionalService,
        ILogger<PerfilController> logger)
    {
        _usuarioService     = usuarioService;
        _profesionalService = profesionalService;
        _logger             = logger;
    }

    // ── USUARIO ──────────────────────────────────────────────

    // GET api/perfil/usuario/5
    [HttpGet("usuario/{id}")]
    public async Task<IActionResult> GetUsuario(int id)
    {
        try
        {
            var usuario = await _usuarioService.BuscarPorIdAsync(id);
            if (usuario is null) return NotFound(new { mensaje = "Usuario no encontrado." });
            return Ok(new {
                usuario.IdUsuario,
                usuario.Nombre,
                usuario.Correo,
                usuario.Matricula,
                usuario.Carrera
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error obteniendo perfil usuario: {msg}", ex.Message);
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }

    // PUT api/perfil/usuario/5
    [HttpPut("usuario/{id}")]
    public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] ActualizarPerfilUsuarioDto dto)
    {
        try
        {
            await _usuarioService.ActualizarPerfilAsync(id, dto);
            return Ok(new { mensaje = "Perfil actualizado correctamente.", nombre = dto.Nombre });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error actualizando perfil usuario: {msg}", ex.Message);
            return StatusCode(500, new { mensaje = "Error al actualizar el perfil." });
        }
    }

    // POST api/perfil/usuario/5/password
    [HttpPost("usuario/{id}/password")]
    public async Task<IActionResult> CambiarPasswordUsuario(int id, [FromBody] CambiarPasswordDto dto)
    {
        try
        {
            var usuario = await _usuarioService.BuscarPorIdAsync(id);
            if (usuario is null) return NotFound(new { mensaje = "Usuario no encontrado." });

            var valido = await _usuarioService.CompararLoginAsync(usuario.Password!, dto.PasswordActual);
            if (!valido) return BadRequest(new { mensaje = "La contraseña actual es incorrecta." });

            await _usuarioService.CambiarPasswordAsync(id, dto.PasswordNuevo);
            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error cambiando password usuario: {msg}", ex.Message);
            return StatusCode(500, new { mensaje = "Error al cambiar la contraseña." });
        }
    }

    // ── PROFESIONAL ───────────────────────────────────────────

    // GET api/perfil/profesional/5
    [HttpGet("profesional/{id}")]
    public async Task<IActionResult> GetProfesional(int id)
    {
        try
        {
            var prof = await _profesionalService.BuscarPorIdAsync(id);
            if (prof is null) return NotFound(new { mensaje = "Profesional no encontrado." });
            return Ok(new {
                prof.IdProfesional,
                prof.Nombre,
                prof.Correo,
                prof.Especialidad,
                prof.HorarioDisponible
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error obteniendo perfil profesional: {msg}", ex.Message);
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }

    // PUT api/perfil/profesional/5
    [HttpPut("profesional/{id}")]
    public async Task<IActionResult> ActualizarProfesional(int id, [FromBody] ActualizarPerfilProfesionalDto dto)
    {
        try
        {
            await _profesionalService.ActualizarPerfilAsync(id, dto);
            return Ok(new { mensaje = "Perfil actualizado correctamente.", nombre = dto.Nombre });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error actualizando perfil profesional: {msg}", ex.Message);
            return StatusCode(500, new { mensaje = "Error al actualizar el perfil." });
        }
    }

    // POST api/perfil/profesional/5/password
    [HttpPost("profesional/{id}/password")]
    public async Task<IActionResult> CambiarPasswordProfesional(int id, [FromBody] CambiarPasswordDto dto)
    {
        try
        {
            var prof = await _profesionalService.BuscarPorIdAsync(id);
            if (prof is null) return NotFound(new { mensaje = "Profesional no encontrado." });

            if (string.IsNullOrEmpty(prof.Password))
                return BadRequest(new { mensaje = "Este perfil no tiene contraseña configurada." });

            var valido = _profesionalService.VerificarPassword(prof.Password, dto.PasswordActual);
            if (!valido) return BadRequest(new { mensaje = "La contraseña actual es incorrecta." });

            await _profesionalService.CambiarPasswordAsync(id, dto.PasswordNuevo);
            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error cambiando password profesional: {msg}", ex.Message);
            return StatusCode(500, new { mensaje = "Error al cambiar la contraseña." });
        }
    }
}
