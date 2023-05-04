using System.Data;
using DocumentStorage.Authentication;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly string? _connectionString;

    public AuthenticationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("documents_postgres");

        if (_connectionString is null) 
        {
            throw new Exception("Connection string must be not null");
        }
    }
    
    public async Task<(int id, string? hash, int role)> GetUserAuthInfoByEmail(string email)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("get_user_auth_info", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_email", NpgsqlDbType.Varchar, 64).Value = email;

        command.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlDbType.Integer));
        command.Parameters.Add(new NpgsqlParameter("p_hash", NpgsqlDbType.Varchar, 100));
        command.Parameters.Add(new NpgsqlParameter("p_role", NpgsqlDbType.Integer));
        command.Parameters["p_id"].Direction = ParameterDirection.Output;
        command.Parameters["p_hash"].Direction = ParameterDirection.Output;
        command.Parameters["p_role"].Direction = ParameterDirection.Output;

        await command.ExecuteNonQueryAsync();

        var id = (command.Parameters["p_id"]?.Value) is null ? 0 : (int)command.Parameters["p_id"].Value!;
        var hash = command.Parameters["p_hash"].Value as string;
        var role = (command.Parameters["p_role"]?.Value) is null ? -1 : ((int)command.Parameters["p_role"]!.Value!);

        return (id, hash, role);
    }
}