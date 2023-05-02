using System.Data;
using Dapper;
using DocumentStorage.Authentication;
using Npgsql;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly string _connectionString;

    public AuthenticationRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
     public async Task<(int, string?, string?)> GetUserAuthInfoByEmail(string email)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("get_user_auth_info", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("email", email);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return (reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
        }

        return (0, null, null);
    }
}