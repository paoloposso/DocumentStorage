using System.Data;
using DocumentStorage.User;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class GroupRepository : IGroupRepository
{
    private readonly string? _connectionString;

    public GroupRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("documents_postgres");

        if (_connectionString is null) 
        {
            throw new Exception("Connection string must be not null");
        }
    }

    public async Task AddGroup(string name, string description)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("add_group", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_name", NpgsqlDbType.Varchar, 50).Value = name;
        command.Parameters.AddWithValue("p_description", NpgsqlDbType.Text).Value = description;

        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<User.User>> ListGroupMembers(int groupId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("list_group_members", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_group_id", NpgsqlDbType.Integer, groupId);

        using var reader = await command.ExecuteReaderAsync();

        var members = new List<User.User>();

        while (await reader.ReadAsync())
        {
            var user = new User.User
            {
                Id = reader.GetInt32(0),
                Email = reader.GetString(1),
                Name = reader.GetString(2)
            };

            members.Add(user);
        }

        return members;
    }

    public async Task<IEnumerable<UserGroup>> ListGroups()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("list_groups", connection);
        command.CommandType = CommandType.StoredProcedure;

        using var reader = await command.ExecuteReaderAsync();

        var groups = new List<UserGroup>();

        while (await reader.ReadAsync())
        {
            var group = new UserGroup
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Description = reader.GetString("description"),
            };

            groups.Add(group);
        }

        return groups;
    }

    public async Task UpdateGroupName(int id, string name)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("update_group_name", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_id", NpgsqlDbType.Integer).Value = id;
        command.Parameters.AddWithValue("p_name", NpgsqlDbType.Varchar, 50).Value = name;

        await command.ExecuteNonQueryAsync();
    }

    public async Task<(bool successful, string? message)> DeleteGroupById(int id)
    {
        string? noticeMessage = null;

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Notice += (sender, args) => noticeMessage = args.Notice.ToString();
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("delete_group_by_id", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_id", NpgsqlDbType.Integer).Value = id;

        await command.ExecuteNonQueryAsync();

        if (string.IsNullOrEmpty(noticeMessage)) 
        {
            return (false, noticeMessage);
        }

        return (true, null);
    }
}