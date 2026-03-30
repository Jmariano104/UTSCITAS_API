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

    public CitasController(CitaService service, CalendarificService calendarific)
    {
        _service = service;
        _calendarific = calendarific;
    }

    // GET api/citas
    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _service.ListarAsync());

    // GET api/citas/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var cita = await _service.BuscarPorIdAsync(id);
        if (cita is null) return NotFound(new { mensaje = "Cita no encontrada." });
        return Ok(cita);
    }

    // GET api/citas/usuario/3
    [HttpGet("usuario/{idUsuario}")]
    public async Task<IActionResult> PorUsuario(int idUsuario)
        => Ok(await _service.PorUsuarioAsync(idUsuario));

    // GET api/citas/profesional/2
    [HttpGet("profesional/{idProfesional}")]
    public async Task<IActionResult> PorProfesional(int idProfesional)
        => Ok(await _service.PorProfesionalAsync(idProfesional));

    // GET api/citas/fecha?inicio=2026-01-01&fin=2026-12-31
    [HttpGet("fecha")]
    public async Task<IActionResult> PorFecha([FromQuery] DateOnly inicio, [FromQuery] DateOnly fin)
        => Ok(await _service.PorFechaAsync(inicio, fin));

    // POST api/citas
    // Valida que la fecha no sea feriado en México antes de crear
    [HttpPost]
    public async Task<IActionResult> Insertar([FromBody] CitaDto dto)
    {
        var esFeriado = await _calendarific.EsFeriadoAsync("MX", dto.Fecha);
        if (esFeriado)
            return BadRequest(new { mensaje = "No se pueden agendar citas en días feriados." });

        var nuevoId = await _service.InsertarAsync(dto);
        return CreatedAtAction(nameof(Buscar), new { id = nuevoId }, new { id = nuevoId });
    }

    // PUT api/citas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarCitaDto dto)
    {
        await _service.ActualizarAsync(id, dto);
        return NoContent();
    }

    // PATCH api/citas/5/estado
    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto)
    {
        await _service.CambiarEstadoAsync(id, dto.IdEstado);
        return NoContent();
    }

    // DELETE api/citas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _service.EliminarAsync(id);
        return NoContent();
    }
}
