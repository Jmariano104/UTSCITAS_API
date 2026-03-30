using Dapper;
using Microsoft.Data.SqlClient;
using UTSCITAS_API.Models;
using UTSCITAS_API.DTOs;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

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
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Usuario>("sp_BuscarUsuarioPorId",
            new { IdUsuario = id },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<int> InsertarAsync(UsuarioDto dto)
    {
        dto.Correo = dto.Correo?.Trim().ToLowerInvariant() ?? string.Empty;

        using var conn = GetConnection();
        try
        {
            return await conn.ExecuteScalarAsync<int>("sp_InsertarUsuario",
                new { dto.Nombre, dto.Correo, dto.Password, dto.Matricula },
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // clave duplicada
        {
            throw new InvalidOperationException("Ya existe un usuario con ese correo.", ex);
        }
    }

    public async Task ActualizarAsync(int id, UsuarioDto dto)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_ActualizarUsuario",
            new { IdUsuario = id, dto.Nombre, dto.Correo, dto.Password, dto.Matricula },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task EliminarAsync(int id)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_EliminarUsuario",
            new { IdUsuario = id },
            commandType: System.Data.CommandType.StoredProcedure);
    }
}

[HttpPost("Crear")]
public async Task<IActionResult> Insertar([FromBody] UsuarioDto dto)
{
    try
    {
        var nuevoId = await _service.InsertarAsync(dto);
        return CreatedAtAction(nameof(Buscar), new { id = nuevoId }, new { id = nuevoId });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new { mensaje = ex.Message });
    }
}
