using System.ComponentModel.DataAnnotations;

namespace UTSCITAS_API.DTOs;

// ── USUARIOS ──────────────────────────────────────────
public class UsuarioDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no es válido")]
    [MaxLength(100)]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(8, ErrorMessage = "Mínimo 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La matrícula es obligatoria")]
    [MaxLength(50)]
    public string Matricula { get; set; } = string.Empty;

    // La BD SÍ tiene columna Carrera
    [MaxLength(100)]
    public string? Carrera { get; set; }
}

// ── PROFESIONALES ──────────────────────────────────────
public class ProfesionalDto
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Correo { get; set; }

    [Required]
    [StringLength(100)]
    public string Especialidad { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? HorarioDisponible { get; set; }
}

// ── CITAS ──────────────────────────────────────────────
// La tabla Citas tiene: IdCita, IdUsuario, IdProfesional, Fecha, TipoCita, Estado(NVARCHAR), Comentario
public class CitaDto
{
    [Required]
    public int IdUsuario { get; set; }

    [Required]
    public int IdProfesional { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    [MaxLength(50)]
    public string TipoCita { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Comentario { get; set; }
}

public class ActualizarCitaDto
{
    public int IdUsuario { get; set; }
    public int IdProfesional { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoCita { get; set; } = string.Empty;
    public string Estado { get; set; } = "Pendiente";

    [MaxLength(500)]
    public string? Comentario { get; set; }
}

public class CambiarEstadoDto
{
    [Required]
    public string Estado { get; set; } = string.Empty;
}

// ── Nuevo DTO para actualizar solo el comentario ────────
public class ActualizarComentarioDto
{
    [MaxLength(500)]
    public string? Comentario { get; set; }
}
