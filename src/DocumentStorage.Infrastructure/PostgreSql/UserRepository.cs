using System.Data;
using System.Text;
using DocumentStorage.User;
using Npgsql;
using NpgsqlTypes;
using Microsoft.Extensions.Configuration;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class UserRepository : IUserRepository
{
    private readonly string? _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("documents_postgres");

        if (_connectionString is null) 
        {
            throw new Exception("Connection string must be not null");
        }
    }

    public async Task UpdateUserRole(string email, string role)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("user_update_role", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_email", email);
        command.Parameters.AddWithValue("p_role", role);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> InsertUser(User.User user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("user_insert", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_email", user.Email);
        command.Parameters.AddWithValue("p_name", user.Name);
        command.Parameters.AddWithValue("p_hashed_password", user.Password);
        command.Parameters.AddWithValue("p_role", NpgsqlDbType.Integer, (int)user.Role);

        var idParameter = new NpgsqlParameter("p_id", NpgsqlDbType.Integer);
        idParameter.Direction = ParameterDirection.Output;
        command.Parameters.Add(idParameter);

        await command.ExecuteNonQueryAsync();

        return Convert.ToInt32(idParameter.Value);
    }

    public async Task AddUserToGroup(int userId, int groupId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("add_user_to_group", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("user_id", userId);
        command.Parameters.AddWithValue("group_id", groupId);

        await command.ExecuteNonQueryAsync();
    }
}
