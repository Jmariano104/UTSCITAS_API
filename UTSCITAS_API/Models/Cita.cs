namespace UTSCITAS_API.Models;

public class Cita
{
    public int IdCita { get; set; }
    public int IdUsuario { get; set; }
    public string? Usuario { get; set; }
    public int IdProfesional { get; set; }
    public string? Profesional { get; set; }
    public string? Especialidad { get; set; }
    public DateTime Fecha { get; set; }
    public DateOnly? FechaSolo { get; set; }
    public string TipoCita { get; set; } = string.Empty;
    public int IdEstado { get; set; }
    public string? Estado { get; set; }
}
