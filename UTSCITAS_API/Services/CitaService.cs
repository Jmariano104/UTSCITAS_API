using Dapper;
using Microsoft.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.DTOs;
using System.Data;

namespace UTSCITAS_API.Services;

public class CitaService
{
    private readonly string _connectionString;

    public CitaService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    private SqlConnection GetConnection() => new SqlConnection(_connectionString);

    // sp_ListarCitas → SELECT * FROM Citas
    public async Task<IEnumerable<Cita>> ListarAsync()
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Cita>("sp_ListarCitas",
            commandType: CommandType.StoredProcedure);
    }

    // sp_BuscarCita → SELECT * FROM Citas WHERE IdCita = @IdCita
    public async Task<Cita?> BuscarPorIdAsync(int id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Cita>("sp_BuscarCita",
            new { IdCita = id },
            commandType: CommandType.StoredProcedure);
    }

    // Filtrar en memoria por usuario (la BD no tiene sp_CitasPorUsuario)
    public async Task<IEnumerable<Cita>> PorUsuarioAsync(int idUsuario)
    {
        var todas = await ListarAsync();
        return todas.Where(c => c.IdUsuario == idUsuario);
    }

    // Filtrar en memoria por profesional
    public async Task<IEnumerable<Cita>> PorProfesionalAsync(int idProfesional)
    {
        var todas = await ListarAsync();
        return todas.Where(c => c.IdProfesional == idProfesional);
    }

    // sp_InsertarCita: @IdUsuario, @IdProfesional, @Fecha, @TipoCita (sin IdEstado)
    public async Task<int> InsertarAsync(CitaDto dto)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_InsertarCita",
            new {
                IdUsuario     = dto.IdUsuario,
                IdProfesional = dto.IdProfesional,
                Fecha         = dto.Fecha,
                TipoCita      = dto.TipoCita
            },
            commandType: CommandType.StoredProcedure);
        return 1;
    }

    // sp_ActualizarCita: @IdCita, @IdUsuario, @IdProfesional, @Fecha, @TipoCita, @Estado
    public async Task ActualizarAsync(int id, ActualizarCitaDto dto)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_ActualizarCita",
            new {
                IdCita        = id,
                IdUsuario     = dto.IdUsuario,
                IdProfesional = dto.IdProfesional,
                Fecha         = dto.Fecha,
                TipoCita      = dto.TipoCita,
                Estado        = dto.Estado
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task CambiarEstadoAsync(int id, string estado)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_ActualizarCita",
            new { IdCita = id, Estado = estado },
            commandType: CommandType.StoredProcedure);
    }

    public async Task EliminarAsync(int id)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_EliminarCita",
            new { IdCita = id },
            commandType: CommandType.StoredProcedure);
    }
}
