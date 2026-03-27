using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace UTSCitas_API.Services
{
    public class CitaService : ICitaService
    {
        private readonly string _connection;
        private readonly IHolidayService _holidayService;
        private readonly ILogger<CitaService> _logger;

        public CitaService(IConfiguration config, IHolidayService holidayService, ILogger<CitaService> logger)
        {
            _connection = config.GetConnectionString("DefaultConnection");
            _holidayService = holidayService;
            _logger = logger;
        }

        public async Task<bool> CrearCita(Cita cita)
        {
            if (cita == null) throw new ArgumentNullException(nameof(cita));

            bool esFestivo = await _holidayService.EsDiaFestivo(cita.Fecha);

            if (esFestivo)
                return false;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connection))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertCita", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = cita.IdUsuario });
                        cmd.Parameters.Add(new SqlParameter("@IdProfesional", SqlDbType.Int) { Value = cita.IdProfesional });
                        cmd.Parameters.Add(new SqlParameter("@Fecha", SqlDbType.DateTime) { Value = cita.Fecha });
                        cmd.Parameters.Add(new SqlParameter("@TipoCita", SqlDbType.NVarChar, 100) { Value = cita.TipoCita });
                        cmd.Parameters.Add(new SqlParameter("@IdEstado", SqlDbType.Int) { Value = cita.IdEstado });

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la cita para Usuario {User} Profesional {Prof} Fecha {Fecha}", cita.IdUsuario, cita.IdProfesional, cita.Fecha);
                throw;
            }
        }
    }
}
