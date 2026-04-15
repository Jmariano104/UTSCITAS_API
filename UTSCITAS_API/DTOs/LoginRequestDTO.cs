namespace UTSCITAS_API.DTOs;

public class LoginRequestDTO
{
    public string Email    { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    // "paciente" o "especialista"
    public string Rol      { get; set; } = "paciente";
}
