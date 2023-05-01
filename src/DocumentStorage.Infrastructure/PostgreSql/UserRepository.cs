using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
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

    public async Task<int> InsertUser(string email, string name, byte[] hashedPassword, byte[] salt, string role)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("user_insert", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_email", email);
        command.Parameters.AddWithValue("p_name", name);
        command.Parameters.AddWithValue("p_hashed_password", hashedPassword);
        command.Parameters.AddWithValue("p_salt", salt);
        command.Parameters.AddWithValue("p_role", role);

        // Add an output parameter to retrieve the ID of the newly inserted user
        var idParameter = new NpgsqlParameter("p_id", NpgsqlDbType.Integer);
        idParameter.Direction = ParameterDirection.Output;
        command.Parameters.Add(idParameter);

        await command.ExecuteNonQueryAsync();

        // Retrieve the ID of the newly inserted user from the output parameter
        return (int)idParameter.Value;
    }


    public async Task<User.User> GetUserByEmail(string email)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("user_get_by_email", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_email", email);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User.User
            {
                Id = reader.GetInt32(0),
                Email = reader.GetString(1),
                Name = reader.GetString(2),
                PasswordSalt = reader.GetFieldValue<byte[]>(3),
                PasswordHash = reader.GetFieldValue<byte[]>(4),
                Role = reader.GetString(5),
                CreatedAt = reader.GetDateTime(6)
            };
        }

        return null;
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
