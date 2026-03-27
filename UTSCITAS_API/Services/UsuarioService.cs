using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using BCryptNet = BCrypt.Net.BCrypt;
using UTSCITAS_API.Models;
using UTSCITAS_API.Services.Interfaces;

namespace UTSCitas_API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string _connection;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IConfiguration config, ILogger<UsuarioService> logger)
        {
            _connection = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task CrearUsuario(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (string.IsNullOrWhiteSpace(usuario.Nombre) || string.IsNullOrWhiteSpace(usuario.Correo) || string.IsNullOrWhiteSpace(usuario.Password))
                throw new ArgumentException("Nombre, Correo y Password son requeridos");

            // Hash password using BCrypt
            var hashed = BCryptNet.HashPassword(usuario.Password);

            try
            {
                using (SqlConnection conn = new SqlConnection(_connection))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertUsuario", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = usuario.Nombre });
                        cmd.Parameters.Add(new SqlParameter("@Correo", SqlDbType.NVarChar, 100) { Value = usuario.Correo });
                        cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 255) { Value = hashed });

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando usuario: {Correo}", usuario.Correo);
                throw; // rethrow so controller can return appropriate response
            }
        }
    }
}