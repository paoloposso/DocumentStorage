using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DocumentStorage.Core;
using Npgsql;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<User?> GetUserByEmail(string email)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("p_email", email, DbType.String, ParameterDirection.Input);
        parameters.Add("id", DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("email", DbType.String, direction: ParameterDirection.Output, size: 255);
        parameters.Add("name", DbType.String, direction: ParameterDirection.Output, size: 50);
        parameters.Add("password", DbType.String, direction: ParameterDirection.Output, size: 255);
        parameters.Add("role", DbType.String, direction: ParameterDirection.Output, size: 10);
        parameters.Add("created_at", DbType.DateTime, direction: ParameterDirection.Output);

        connection.Execute("user_get_by_email", parameters, commandType: CommandType.StoredProcedure);

        if (parameters.Get<int>("id").Equals(0))
        {
            return null;
        }

        // var user = new User(
        //     id: parameters.Get<int>("id"),
        //     email: parameters.Get<string>("email"),
        //     name: parameters.Get<string>("name"),
        //     // password: parameters.Get<string>("password"),
        //     role: parameters.Get<string>("role"),
        //     // createdAt: parameters.Get<DateTime>("created_at")
        // );

        // return user;

        return null;
    }
}