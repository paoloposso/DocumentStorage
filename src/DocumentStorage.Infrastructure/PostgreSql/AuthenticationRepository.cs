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
    
    public async Task<(string id, string hashedPassword)?> GetUserByEmail(string email)
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

        var id = parameters.Get<string>("id");
        var hashedPassword = parameters.Get<string>("password");

        return (id, hashedPassword);
    }
}