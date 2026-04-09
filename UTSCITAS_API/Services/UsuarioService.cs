using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.DTOs;
using System.Data;

namespace UTSCITAS_API.Services;

public class UsuarioService
{
    private readonly string _connectionString;

    public UsuarioService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    private SqlConnection GetConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Usuario>> ListarAsync()
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Usuario>("sp_ListarUsuarios",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Usuario>("sp_BuscarUsuario",
            new { IdUsuario = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> InsertarAsync(UsuarioDto dto)
    {
        dto.Correo = dto.Correo?.Trim().ToLowerInvariant() ?? string.Empty;

        // Hashear password con PBKDF2
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(dto.Password), salt, 10000, HashAlgorithmName.SHA256, 32);
        var passwordStored = Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);

        using var conn = GetConnection();
        try
        {
            // La BD tiene: Nombre, Correo, Matricula, Carrera, Password
            await conn.ExecuteAsync("sp_InsertarUsuario",
                new {
                    Nombre    = dto.Nombre,
                    Correo    = dto.Correo,
                    Matricula = dto.Matricula,
                    Carrera   = dto.Carrera ?? string.Empty,
                    Password  = passwordStored
                },
                commandType: CommandType.StoredProcedure);
            return 1; // SP no retorna ID, regresamos 1 como éxito
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601 || ex.Number == 50000)
        {
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    public async Task ActualizarAsync(int id, UsuarioDto dto)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_ActualizarUsuario",
            new { IdUsuario = id, dto.Nombre, dto.Correo, dto.Matricula, dto.Carrera, dto.Password },
            commandType: CommandType.StoredProcedure);
    }

    public async Task EliminarAsync(int id)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_EliminarUsuario",
            new { IdUsuario = id },
            commandType: CommandType.StoredProcedure);
    }

    internal async Task<Usuario?> BuscarPorCorreoAsync(string correo)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Usuario>("sp_BuscarUsuarioPorCorreo",
            new { Correo = correo },
            commandType: CommandType.StoredProcedure);
    }

    internal async Task<bool> CompararLoginAsync(string passwordDb, string passwordIngresado)
    {
        var parts = passwordDb?.Split('.');
        if (parts == null || parts.Length != 2) return false;
        var salt        = Convert.FromBase64String(parts[0]);
        var hashStored  = Convert.FromBase64String(parts[1]);
        var hashCompare = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(passwordIngresado), salt, 10000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(hashStored, hashCompare);
    }
}
