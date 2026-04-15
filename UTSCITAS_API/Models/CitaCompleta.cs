namespace UTSCITAS_API.Models;

// Modelo enriquecido para el panel del especialista
public class CitaCompleta
{
    public int    IdCita             { get; set; }
    public int    IdUsuario          { get; set; }
    public string NombreUsuario      { get; set; } = string.Empty;
    public string CorreoUsuario      { get; set; } = string.Empty;
    public string Matricula          { get; set; } = string.Empty;
    public int    IdProfesional      { get; set; }
    public string NombreProfesional  { get; set; } = string.Empty;
    public string Especialidad       { get; set; } = string.Empty;
    public DateTime Fecha            { get; set; }
    public string TipoCita           { get; set; } = string.Empty;
    public string Estado             { get; set; } = "Pendiente";
}
