using System.Data;
using DocumentStorage.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class GroupRepository : IGroupRepository
{
    private readonly string? _connectionString;
    private readonly ILogger<GroupRepository> _logger;

    public GroupRepository(ILogger<GroupRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
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
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Name = reader.GetString(reader.GetOrdinal("name"))
            };

            members.Add(user);
        }

        return members;
    }

    public async Task<IEnumerable<UserGroup>> ListGroups()
    {
        var groups = new List<UserGroup>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new NpgsqlCommand("SELECT * FROM list_groups()", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var group = new UserGroup
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Description = reader.GetString(reader.GetOrdinal("description"))
                    };
                    groups.Add(group);
                }
            }
        }

        return groups;
    }


    public async Task Update(UserGroup group)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("update_group", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_id", NpgsqlDbType.Integer).Value = group.Id;
        command.Parameters.AddWithValue("p_name", NpgsqlDbType.Varchar, 50).Value = group.Name;
        command.Parameters.AddWithValue("p_description", NpgsqlDbType.Varchar, 50).Value = group.Name;

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

        if (!string.IsNullOrEmpty(noticeMessage)) 
        {
            return (false, noticeMessage);
        }

        return (true, null);
    }

    public async Task<UserGroup?> GetGroupById(int groupId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "get_group_by_id";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@p_id", groupId);

        command.Parameters.Add("p_name", NpgsqlDbType.Varchar, 50).Direction = ParameterDirection.Output;
        command.Parameters.Add("p_description", NpgsqlDbType.Text).Direction = ParameterDirection.Output;

        try
        {
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                if (Convert.IsDBNull(command.Parameters["p_name"].Value))
                {
                    return null;
                }

                var name = reader.GetString("p_name");
                var description = reader.GetString("p_description");

                return new UserGroup
                {
                    Id = groupId,
                    Name = name,
                    Description = description
                };
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting group with ID {GroupId}", groupId);
            throw;
        }
    }

}