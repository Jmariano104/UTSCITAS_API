namespace UTSCITAS_API.Models
{
    public class Cita
    {
        public int IdCita { get; set; }

        public int IdUsuario { get; set; }

        public int IdProfesional { get; set; }

        public DateTime Fecha { get; set; }

        public string TipoCita { get; set; }

        // En la base de datos la tabla Citas referencia a EstadosCita mediante IdEstado (int).
        // Usar IdEstado aquí para mantener consistencia con la BD y las FK.
        public int IdEstado { get; set; }
    }
}