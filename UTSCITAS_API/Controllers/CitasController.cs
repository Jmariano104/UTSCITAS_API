using Microsoft.AspNetCore.Mvc;
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

    // GET api/citas
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        try { return Ok(await _service.ListarAsync()); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    // GET api/citas/5
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

    // GET api/citas/usuario/3
    [HttpGet("usuario/{idUsuario}")]
    public async Task<IActionResult> PorUsuario(int idUsuario)
    {
        try { return Ok(await _service.PorUsuarioAsync(idUsuario)); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    // GET api/citas/profesional/2
    [HttpGet("profesional/{idProfesional}")]
    public async Task<IActionResult> PorProfesional(int idProfesional)
    {
        try { return Ok(await _service.PorProfesionalAsync(idProfesional)); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    // POST api/citas
    [HttpPost]
    public async Task<IActionResult> Insertar([FromBody] CitaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            // Verificar feriado — si Calendarific falla, se permite la cita igualmente
            try
            {
                var esFeriado = await _calendarific.EsFeriadoAsync("MX", dto.Fecha);
                if (esFeriado)
                    return BadRequest(new { mensaje = "No se pueden agendar citas en días feriados." });
            }
            catch (Exception ex)
            {
                // API key inválida o sin conexión — se ignora y se permite la cita
                _logger.LogWarning("Calendarific no disponible: {msg}. Se permite la cita.", ex.Message);
            }

            await _service.InsertarAsync(dto);
            return Ok(new { mensaje = "Cita agendada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al insertar cita: {msg}", ex.Message);
            return StatusCode(500, new { mensaje = ex.Message });
        }
    }

    // PUT api/citas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarCitaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try { await _service.ActualizarAsync(id, dto); return NoContent(); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    // PATCH api/citas/5/estado
    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto)
    {
        try { await _service.CambiarEstadoAsync(id, dto.Estado); return NoContent(); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }

    // DELETE api/citas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try { await _service.EliminarAsync(id); return NoContent(); }
        catch (Exception) { return StatusCode(500, new { mensaje = "Error interno del servidor." }); }
    }
}
