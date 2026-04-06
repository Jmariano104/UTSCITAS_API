using Microsoft.AspNetCore.Mvc;
using UTSCITAS_API.Services;

namespace UTSCITAS_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeriadosController : ControllerBase
{
    private readonly CalendarificService _service;

    public FeriadosController(CalendarificService service) => _service = service;

    // GET api/feriados?country=MX&year=2026
    [HttpGet]
    public async Task<IActionResult> ObtenerFeriados(
        [FromQuery] string country = "MX",
        [FromQuery] int? year = null)
    {
        year ??= DateTime.Now.Year;
        var feriados = await _service.ObtenerFeriadosAsync(country, year.Value);
        return Ok(feriados);
    }

    // GET api/feriados/mes?country=MX&year=2026&month=3
    [HttpGet("mes")]
    public async Task<IActionResult> FeriadosPorMes(
        [FromQuery] string country = "MX",
        [FromQuery] int? year = null,
        [FromQuery] int month = 1)
    {
        year ??= DateTime.Now.Year;
        var feriados = await _service.FeriadosPorMesAsync(country, year.Value, month);
        return Ok(feriados);
    }

    // GET api/feriados/verificar?country=MX&fecha=2026-05-01
    [HttpGet("verificar")]
    public async Task<IActionResult> VerificarFeriado(
        [FromQuery] string country = "MX",
        [FromQuery] DateTime fecha = default)
    {
        if (fecha == default) fecha = DateTime.Today;
        var esFeriado = await _service.EsFeriadoAsync(country, fecha);
        return Ok(new { fecha = fecha.ToString("yyyy-MM-dd"), esFeriado });
    }
}
