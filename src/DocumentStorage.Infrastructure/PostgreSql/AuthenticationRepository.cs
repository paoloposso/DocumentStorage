using System.Data;
using Dapper;
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
    
    public async Task<(int, string?)> GetUserAuthInfoByEmail(string email)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("get_user_auth_info", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_email", NpgsqlDbType.Varchar, 64).Value = email;

        command.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlDbType.Integer));
        command.Parameters.Add(new NpgsqlParameter("p_hash", NpgsqlDbType.Varchar, 100));
        command.Parameters["p_id"].Direction = ParameterDirection.Output;
        command.Parameters["p_hash"].Direction = ParameterDirection.Output;

        await command.ExecuteNonQueryAsync();

        var id = (command.Parameters["p_id"]?.Value) is null ? 0 : (int)command.Parameters["p_id"].Value!;
        var hash = command.Parameters["p_hash"].Value as string;

        return (id, hash);
    }
}