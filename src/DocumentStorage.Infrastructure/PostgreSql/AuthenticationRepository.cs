using System.Data;
using DocumentStorage.Authentication;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class AuthenticationRepository : BasePostgresRepository, IAuthenticationRepository
{
    public AuthenticationRepository(IConfiguration configuration) : base(configuration) {}
    
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

        if (command.Parameters["p_id"]?.Value is System.DBNull) 
        {
            return (0, null, -1);
        }

        var id = (command.Parameters["p_id"]?.Value) is System.DBNull ? 0 : (int)command.Parameters["p_id"].Value!;
        var hash = command.Parameters["p_hash"].Value as string;
        var role = (command.Parameters["p_role"]?.Value) is System.DBNull ? -1 : ((int)command.Parameters["p_role"]!.Value!);

        return (id, hash, role);
    }
}