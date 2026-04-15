using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.DTOs;
using System.Data;

namespace UTSCITAS_API.Services;

public class ProfesionalService
{
    private readonly string _connectionString;

    public ProfesionalService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    private SqlConnection GetConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Profesional>> ListarAsync()
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Profesional>("sp_ListarProfesionales",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Profesional?> BuscarPorIdAsync(int id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Profesional>("sp_BuscarProfesional",
            new { IdProfesional = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Profesional?> BuscarPorCorreoAsync(string correo)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Profesional>("sp_BuscarProfesionalPorCorreo",
            new { Correo = correo },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<CitaCompleta>> ListarCitasCompletasAsync()
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<CitaCompleta>("sp_ListarCitasCompleto",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<CitaCompleta>> CitasPorProfesionalAsync(int idProfesional)
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<CitaCompleta>("sp_CitasPorProfesionalCompleto",
            new { IdProfesional = idProfesional },
            commandType: CommandType.StoredProcedure);
    }

    public async Task CambiarEstadoCitaAsync(int idCita, string estado)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_CambiarEstadoCita",
            new { IdCita = idCita, Estado = estado },
            commandType: CommandType.StoredProcedure);
    }

    public async Task EliminarCitaAsync(int idCita)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_EliminarCita",
            new { IdCita = idCita },
            commandType: CommandType.StoredProcedure);
    }

    // Actualizar solo nombre y horario (perfil)
    public async Task ActualizarPerfilAsync(int id, ActualizarPerfilProfesionalDto dto)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_ActualizarPerfilProfesional",
            new { IdProfesional = id, dto.Nombre, dto.HorarioDisponible },
            commandType: CommandType.StoredProcedure);
    }

    // Cambiar contraseña
    public async Task CambiarPasswordAsync(int id, string nuevaPassword)
    {
        var hash = HashPassword(nuevaPassword);
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_CambiarPasswordProfesional",
            new { IdProfesional = id, Password = hash },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> InsertarAsync(ProfesionalDto dto)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_InsertarProfesional",
            new { dto.Nombre, dto.Correo, dto.Especialidad, dto.HorarioDisponible },
            commandType: CommandType.StoredProcedure);
        return 1;
    }

    public async Task ActualizarAsync(int id, ProfesionalDto dto)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_ActualizarProfesional",
            new { IdProfesional = id, dto.Nombre, dto.Correo, dto.Especialidad, dto.HorarioDisponible },
            commandType: CommandType.StoredProcedure);
    }

    public async Task EliminarAsync(int id)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_EliminarProfesional",
            new { IdProfesional = id },
            commandType: CommandType.StoredProcedure);
    }

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password), salt, 10000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    public bool VerificarPassword(string passwordHash, string passwordIngresado)
    {
        var parts = passwordHash?.Split('.');
        if (parts == null || parts.Length != 2) return false;
        var salt        = Convert.FromBase64String(parts[0]);
        var hashStored  = Convert.FromBase64String(parts[1]);
        var hashCompare = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(passwordIngresado), salt, 10000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(hashStored, hashCompare);
    }
}
