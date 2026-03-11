using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.Services.Interfaces;

namespace UTSCitas_API.Services
{
    public class ProfesionalService : IProfesionalService
    {
        private readonly string _connection;

        public ProfesionalService(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Profesional>> ObtenerProfesionales()
        {
            List<Profesional> lista = new List<Profesional>();

            using (SqlConnection conn = new SqlConnection(_connection))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Profesionales", conn);

                await conn.OpenAsync();

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    lista.Add(new Profesional
                    {
                        IdProfesional = (int)reader["IdProfesional"],
                        Nombre = reader["Nombre"].ToString(),
                        Especialidad = reader["Especialidad"].ToString()
                    });
                }
            }

            return lista;
        }
    }
}