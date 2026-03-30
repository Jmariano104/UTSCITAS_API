namespace UTSCITAS_API.DTOs;

public class UsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
}

public class ProfesionalDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
}

public class CitaDto
{
    public int IdUsuario { get; set; }
    public int IdProfesional { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoCita { get; set; } = string.Empty;
    public int IdEstado { get; set; } = 1;
}

public class ActualizarCitaDto
{
    public int IdProfesional { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoCita { get; set; } = string.Empty;
    public int IdEstado { get; set; }
}

public class CambiarEstadoDto
{
    public int IdEstado { get; set; }
}

public class EstadoCitaDto
{
    public string Nombre { get; set; } = string.Empty;
}
