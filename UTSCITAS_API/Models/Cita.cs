namespace UTSCITAS_API.Models;

public class Cita
{
    public int IdCita { get; set; }
    public int IdUsuario { get; set; }
    public int IdProfesional { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoCita { get; set; } = string.Empty;
    // La BD usa Estado como texto, NO IdEstado
    public string Estado { get; set; } = "Pendiente";
}
