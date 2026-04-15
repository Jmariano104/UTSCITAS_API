namespace UTSCITAS_API.DTOs;

public class ActualizarPerfilUsuarioDto
{
    public string Nombre  { get; set; } = string.Empty;
    public string? Carrera { get; set; }
}

public class ActualizarPerfilProfesionalDto
{
    public string Nombre            { get; set; } = string.Empty;
    public string? HorarioDisponible { get; set; }
}

public class CambiarPasswordDto
{
    public string PasswordActual { get; set; } = string.Empty;
    public string PasswordNuevo  { get; set; } = string.Empty;
}
