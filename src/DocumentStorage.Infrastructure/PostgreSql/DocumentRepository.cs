using System.Data;
using Dapper;
using DocumentStorage.Document;
using Npgsql;
using NpgsqlTypes;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class DocumentRepository : IDocumentRepository
{
    private readonly string _connectionString;

    public DocumentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<DocumentMetadata?> GetDocumentByIdForUser(int documentId, int userId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        
        connection.Open();
        
        using var command = new NpgsqlCommand("get_document_by_id_for_user", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_document_id", documentId);
        command.Parameters.AddWithValue("p_user_id", userId);

        command.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });
        command.Parameters.Add(new NpgsqlParameter("p_name", NpgsqlDbType.Varchar) { Size = 255, Direction = ParameterDirection.Output });
        command.Parameters.Add(new NpgsqlParameter("p_description", NpgsqlDbType.Text) { Direction = ParameterDirection.Output });
        command.Parameters.Add(new NpgsqlParameter("p_file_path", NpgsqlDbType.Text) { Direction = ParameterDirection.Output });
        command.Parameters.Add(new NpgsqlParameter("p_created_by", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });
        command.Parameters.Add(new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp) { Direction = ParameterDirection.Output });

        await command.ExecuteNonQueryAsync();

        if (command.Parameters["p_id"].Value is null)
        {
            return null;
        }

        int id = Convert.ToInt32(command.Parameters["p_id"].Value);
        string name = Convert.ToString(command.Parameters["p_name"].Value);
        string description = Convert.ToString(command.Parameters["p_description"].Value);
        string filePath = Convert.ToString(command.Parameters["p_file_path"]?.Value);
        int createdBy = Convert.ToInt32(command.Parameters["p_created_by"].Value);
        DateTime createdAt = Convert.ToDateTime(command.Parameters["p_created_at"].Value);

        return new DocumentMetadata
        {
            Id = id,
            Name = name ?? string.Empty,
            Description = description ?? string.Empty,
            FilePath = filePath ?? string.Empty,
            CreatedByUser = createdBy,
            PostedDate = createdAt
        };  
    }

    public async Task InsertDocumentMetadata(DocumentMetadata document)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        
        await connection.OpenAsync();

        using var command = new NpgsqlCommand("document_insert", connection);

        command.Parameters.AddWithValue("p_name", document.Name);
        command.Parameters.AddWithValue("p_description", document.Description);
        command.Parameters.AddWithValue("p_file_path", document.FilePath);
        command.Parameters.AddWithValue("p_created_by", document.CreatedByUser);

        await command.ExecuteNonQueryAsync();
    }
}
