using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.Services.Interfaces;

namespace UTSCitas_API.Services
{
    public class CitaService : ICitaService
    {
        private readonly string _connection;
        private readonly IHolidayService _holidayService;

        public CitaService(IConfiguration config, IHolidayService holidayService)
        {
            _connection = config.GetConnectionString("DefaultConnection");
            _holidayService = holidayService;
        }

        public async Task<bool> CrearCita(Cita cita)
        {
            bool esFestivo = await _holidayService.EsDiaFestivo(cita.Fecha);

            if (esFestivo)
                return false;

            using (SqlConnection conn = new SqlConnection(_connection))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertCita", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", cita.IdUsuario);
                    cmd.Parameters.AddWithValue("@IdProfesional", cita.IdProfesional);
                    cmd.Parameters.AddWithValue("@Fecha", cita.Fecha);
                    cmd.Parameters.AddWithValue("@TipoCita", cita.TipoCita);
                    cmd.Parameters.AddWithValue("@Estado", cita.Estado);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return true;
        }
    }
}
