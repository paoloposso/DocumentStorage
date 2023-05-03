using System.Data;
using System.Text;
using DocumentStorage.User;
using Npgsql;
using NpgsqlTypes;
using Microsoft.Extensions.Configuration;
using DocumentStorage.Core;
using Microsoft.Extensions.Logging;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class UserRepository : IUserRepository
{
    private readonly string? _connectionString;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("documents_postgres");

        if (_connectionString is null) 
        {
            throw new Exception("Connection string must be not null");
        }
    }

    public async Task UpdateUser(int id, Role role, bool active)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("update_user", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_user_id", NpgsqlDbType.Integer, id);
        command.Parameters.AddWithValue("p_active", NpgsqlDbType.Boolean, active);
        command.Parameters.AddWithValue("p_user_role", NpgsqlDbType.Integer, (int)role);

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

    public async Task<IEnumerable<User.User>> ListUsers()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("list_users", connection);
        command.CommandType = CommandType.StoredProcedure;

        using var reader = await command.ExecuteReaderAsync();

        var users = new List<User.User>();

        while (await reader.ReadAsync())
        {
            var user = new User.User
            {
                Id = reader.GetInt32("id"),
                Email = reader.GetString("email"),
                Name = reader.GetString("name"),
                Role = (Role)reader.GetInt32("user_role"),
                Active = reader.GetBoolean("active")
            };

            users.Add(user);
        }

        return users;
    }

    public async Task AddUserToGroup(int userId, int groupId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Register the Notification event handler before opening the connection
        connection.Notification += (sender, args) =>
        {
            if (args.Channel.Equals("NOTICE"))
            {
                _logger.LogWarning(args.Payload);
            }
        };

        using var command = new NpgsqlCommand("add_user_to_group", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_user_id", NpgsqlDbType.Integer, userId);
        command.Parameters.AddWithValue("p_group_id", NpgsqlDbType.Integer, groupId);

        await command.ExecuteNonQueryAsync();
    }
}
