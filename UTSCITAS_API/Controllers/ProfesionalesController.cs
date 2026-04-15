using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services;
using UTSCITAS_API.DTOs;

namespace UTSCITAS_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfesionalesController : ControllerBase
{
    private readonly ProfesionalService _service;
    private readonly ILogger<ProfesionalesController> _logger;

    public ProfesionalesController(ProfesionalService service, ILogger<ProfesionalesController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    // GET api/profesionales
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        try { return Ok(await _service.ListarAsync()); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno." }); }
    }

    // GET api/profesionales/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        try
        {
            var p = await _service.BuscarPorIdAsync(id);
            if (p is null) return NotFound(new { mensaje = "Profesional no encontrado." });
            return Ok(p);
        }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno." }); }
    }

    // POST api/profesionales/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        try
        {
            var profesional = await _service.BuscarPorCorreoAsync(dto.Email);
            if (profesional is null)
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });

            if (string.IsNullOrEmpty(profesional.Password))
                return Unauthorized(new { mensaje = "Este especialista no tiene contraseña configurada. Contacta al administrador." });

            var valido = _service.VerificarPassword(profesional.Password, dto.Password);
            if (!valido)
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });

            return Ok(new
            {
                mensaje        = "Login exitoso.",
                idProfesional  = profesional.IdProfesional,
                nombre         = profesional.Nombre,
                correo         = profesional.Correo,
                especialidad   = profesional.Especialidad,
                rol            = "especialista"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error en login profesional: {msg}", ex.Message);
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }

    // GET api/profesionales/citas — todas las citas (panel especialista)
    [HttpGet("citas")]
    public async Task<IActionResult> TodasLasCitas()
    {
        try { return Ok(await _service.ListarCitasCompletasAsync()); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno." }); }
    }

    // GET api/profesionales/{id}/citas — citas de un profesional específico
    [HttpGet("{id}/citas")]
    public async Task<IActionResult> CitasPorProfesional(int id)
    {
        try { return Ok(await _service.CitasPorProfesionalAsync(id)); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno." }); }
    }

    // PATCH api/profesionales/citas/{idCita}/estado
    [HttpPatch("citas/{idCita}/estado")]
    public async Task<IActionResult> CambiarEstado(int idCita, [FromBody] CambiarEstadoDto dto)
    {
        try
        {
            await _service.CambiarEstadoCitaAsync(idCita, dto.Estado);
            return Ok(new { mensaje = $"Estado actualizado a '{dto.Estado}'." });
        }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno." }); }
    }

    // DELETE api/profesionales/citas/{idCita}
    [HttpDelete("citas/{idCita}")]
    public async Task<IActionResult> EliminarCita(int idCita)
    {
        try
        {
            await _service.EliminarCitaAsync(idCita);
            return Ok(new { mensaje = "Cita eliminada." });
        }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno." }); }
    }

    // POST api/profesionales
    [HttpPost]
    public async Task<IActionResult> Insertar([FromBody] ProfesionalDto dto)
    {
        try { await _service.InsertarAsync(dto); return Ok(new { mensaje = "Profesional creado." }); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno." }); }
    }

    // PUT api/profesionales/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ProfesionalDto dto)
    {
        try { await _service.ActualizarAsync(id, dto); return NoContent(); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno." }); }
    }

    // DELETE api/profesionales/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try { await _service.EliminarAsync(id); return NoContent(); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno." }); }
    }
}
