using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services;
using UTSCITAS_API.DTOs;
namespace UTSCITAS_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfesionalesController : ControllerBase
{
    private readonly ProfesionalService _service;

    public ProfesionalesController(ProfesionalService service) => _service = service;

    // GET api/profesionales
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        try
        {
            return Ok(await _service.ListarAsync());
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }

    // GET api/profesionales/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        try
        {
            var profesional = await _service.BuscarPorIdAsync(id);
            if (profesional is null) return NotFound(new { mensaje = "Profesional no encontrado." });
            return Ok(profesional);
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }

    // POST api/profesionales
    [HttpPost]
    public async Task<IActionResult> Insertar([FromBody] ProfesionalDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var nuevoId = await _service.InsertarAsync(dto);
            return CreatedAtAction(nameof(Buscar), new { id = nuevoId }, new { id = nuevoId });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }

    // PUT api/profesionales/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ProfesionalDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _service.ActualizarAsync(id, dto);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }

    // DELETE api/profesionales/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }
}
