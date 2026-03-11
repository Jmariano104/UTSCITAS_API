using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.Services.Interfaces;

namespace UTSCitas_API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string _connection;

        public UsuarioService(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }

        public async Task CrearUsuario(Usuario usuario)
        {
            using (SqlConnection conn = new SqlConnection(_connection))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@Password", usuario.Password);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}