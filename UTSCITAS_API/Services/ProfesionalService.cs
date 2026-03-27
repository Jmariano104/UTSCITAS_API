using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using UTSCITAS_API.Models;
using UTSCITAS_API.Services.Interfaces;

namespace UTSCitas_API.Services
{
    public class ProfesionalService : IProfesionalService
    {
        private readonly string _connection;
        private readonly ILogger<ProfesionalService> _logger;

        public ProfesionalService(IConfiguration config, ILogger<ProfesionalService> logger)
        {
            _connection = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<List<Profesional>> ObtenerProfesionales()
        {
            List<Profesional> lista = new List<Profesional>();

            using (SqlConnection conn = new SqlConnection(_connection))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT IdProfesional, Nombre, Especialidad FROM Profesionales", conn);

                    await conn.OpenAsync();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        lista.Add(new Profesional
                        {
                            IdProfesional = reader.GetInt32(0),
                            Nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Especialidad = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error obteniendo profesionales");
                    throw;
                }
            }

            return lista;
        }
    }
}