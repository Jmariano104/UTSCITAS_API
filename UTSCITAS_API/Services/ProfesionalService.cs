using Dapper;
using Microsoft.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.DTOs;

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
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<Profesional?> BuscarPorIdAsync(int id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Profesional>("sp_BuscarProfesionalPorId",
            new { IdProfesional = id },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<int> InsertarAsync(ProfesionalDto dto)
    {
        using var conn = GetConnection();
        return await conn.ExecuteScalarAsync<int>("sp_InsertarProfesional",
            new { dto.Nombre, dto.Especialidad },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task ActualizarAsync(int id, ProfesionalDto dto)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_ActualizarProfesional",
            new { IdProfesional = id, dto.Nombre, dto.Especialidad },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task EliminarAsync(int id)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_EliminarProfesional",
            new { IdProfesional = id },
            commandType: System.Data.CommandType.StoredProcedure);
    }
}
