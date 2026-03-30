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
        => Ok(await _service.ListarAsync());

    // GET api/profesionales/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var profesional = await _service.BuscarPorIdAsync(id);
        if (profesional is null) return NotFound(new { mensaje = "Profesional no encontrado." });
        return Ok(profesional);
    }

    // POST api/profesionales
    [HttpPost]
    public async Task<IActionResult> Insertar([FromBody] ProfesionalDto dto)
    {
        var nuevoId = await _service.InsertarAsync(dto);
        return CreatedAtAction(nameof(Buscar), new { id = nuevoId }, new { id = nuevoId });
    }

    // PUT api/profesionales/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ProfesionalDto dto)
    {
        await _service.ActualizarAsync(id, dto);
        return NoContent();
    }

    // DELETE api/profesionales/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _service.EliminarAsync(id);
        return NoContent();
    }
}
