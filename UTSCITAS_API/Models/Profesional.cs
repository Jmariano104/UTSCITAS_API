namespace UTSCITAS_API.Models;

public class Profesional
{
    public int IdProfesional { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string Especialidad { get; set; } = string.Empty;
    public string? HorarioDisponible { get; set; }
    public DateTime? FechaRegistro { get; set; }
}
