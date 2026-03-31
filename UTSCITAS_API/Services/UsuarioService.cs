using System;
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
        return await conn.QueryFirstOrDefaultAsync<Usuario>("sp_BuscarUsuarioPorId",
            new { IdUsuario = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> InsertarAsync(UsuarioDto dto)
    {
        dto.Correo = dto.Correo?.Trim().ToLowerInvariant() ?? string.Empty;

        // Hashear password (simple PBKDF2)
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(dto.Password), salt, 10000, HashAlgorithmName.SHA256, 32);
        var passwordStored = Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);

        using var conn = GetConnection();
        try
        {
            return await conn.ExecuteScalarAsync<int>("sp_InsertarUsuario",
                new { Matricula = dto.Matricula,Nombre = dto.Nombre, Correo = dto.Correo, Password = passwordStored },
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
            new { IdUsuario = id, dto.Matricula,dto.Nombre, dto.Correo, dto.Password },
            commandType: CommandType.StoredProcedure);
    }

    public async Task EliminarAsync(int id)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync("sp_EliminarUsuario",
            new { IdUsuario = id },
            commandType: CommandType.StoredProcedure);
    }
}

