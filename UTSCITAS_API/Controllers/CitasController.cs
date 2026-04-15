using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using UTSCITAS_API.Services;
using UTSCITAS_API.DTOs;

namespace UTSCITAS_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly CitaService _service;
    private readonly CalendarificService _calendarific;
    private readonly ILogger<CitasController> _logger;

    public CitasController(CitaService service, CalendarificService calendarific, ILogger<CitasController> logger)
    {
        _service      = service;
        _calendarific = calendarific;
        _logger       = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        try { return Ok(await _service.ListarAsync()); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        try
        {
            var cita = await _service.BuscarPorIdAsync(id);
            if (cita is null) return NotFound(new { mensaje = "Cita no encontrada." });
            return Ok(cita);
        }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    [HttpGet("usuario/{idUsuario}")]
    public async Task<IActionResult> PorUsuario(int idUsuario)
    {
        try { return Ok(await _service.PorUsuarioAsync(idUsuario)); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    [HttpGet("profesional/{idProfesional}")]
    public async Task<IActionResult> PorProfesional(int idProfesional)
    {
        try { return Ok(await _service.PorProfesionalAsync(idProfesional)); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    [HttpPost]
    public async Task<IActionResult> Insertar([FromBody] CitaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            // Calendarific opcional — si falla se permite la cita
            try
            {
                var esFeriado = await _calendarific.EsFeriadoAsync("MX", dto.Fecha);
                if (esFeriado)
                    return BadRequest(new { mensaje = "No se pueden agendar citas en días feriados." });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Calendarific no disponible: {msg}", ex.Message);
            }

            await _service.InsertarAsync(dto);
            return Ok(new { mensaje = "Cita agendada exitosamente." });
        }
        catch (SqlException ex) when (ex.Class == 16)
        {
            // RAISERROR con severidad 16 = error de regla de negocio (validación)
            _logger.LogWarning("Validación al insertar cita: {msg}", ex.Message);
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al insertar cita: {msg}", ex.Message);
            return StatusCode(500, new { mensaje = "Error al agendar la cita. Inténtalo de nuevo." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarCitaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try { await _service.ActualizarAsync(id, dto); return NoContent(); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto)
    {
        try { await _service.CambiarEstadoAsync(id, dto.Estado); return NoContent(); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    // PATCH api/citas/{id}/comentario — actualiza solo el comentario de la cita
    [HttpPatch("{id}/comentario")]
    public async Task<IActionResult> ActualizarComentario(int id, [FromBody] ActualizarComentarioDto dto)
    {
        try
        {
            await _service.ActualizarComentarioAsync(id, dto.Comentario);
            return Ok(new { mensaje = "Comentario actualizado correctamente." });
        }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    // DELETE api/citas/{id} — el usuario puede cancelar/eliminar su propia cita
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _service.EliminarAsync(id);
            return Ok(new { mensaje = "Cita eliminada correctamente." });
        }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }
}
