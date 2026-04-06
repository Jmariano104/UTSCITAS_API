using Dapper;
using Microsoft.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.DTOs;

namespace UTSCITAS_API.Services;

public class CitaService
{
    private readonly string _connectionString;

    public CitaService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    private SqlConnection GetConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Cita>> ListarAsync()
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Cita>("sp_ListarCitas",
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<Cita?> BuscarPorIdAsync(int id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Cita>("sp_BuscarCitaPorId",
            new { IdCita = id },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Cita>> PorUsuarioAsync(int idUsuario)
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Cita>("sp_CitasPorUsuario",
            new { IdUsuario = idUsuario },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Cita>> PorProfesionalAsync(int idProfesional)
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Cita>("sp_CitasPorProfesional",
            new { IdProfesional = idProfesional },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Cita>> PorFechaAsync(DateOnly inicio, DateOnly fin)
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Cita>("sp_CitasPorFecha",
            new { FechaInicio = inicio, FechaFin = fin },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<int> InsertarAsync(CitaDto dto)
    {
        using var conn = GetConnection();
        return await conn.ExecuteScalarAsync<int>("sp_InsertarCita",
            new { dto.IdUsuario, dto.IdProfesional, dto.Fecha, dto.TipoCita, dto.IdEstado },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task ActualizarAsync(int id, ActualizarCitaDto dto)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_ActualizarCita",
            new { IdCita = id, dto.IdProfesional, dto.Fecha, dto.TipoCita, dto.IdEstado },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task CambiarEstadoAsync(int id, int idEstado)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_CambiarEstadoCita",
            new { IdCita = id, IdEstado = idEstado },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task EliminarAsync(int id)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_EliminarCita",
            new { IdCita = id },
            commandType: System.Data.CommandType.StoredProcedure);
    }
}
