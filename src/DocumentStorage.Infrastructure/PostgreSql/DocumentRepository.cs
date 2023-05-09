using System;
using System.Data;
using Dapper;
using DocumentStorage.Document;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class DocumentRepository : BasePostgresRepository, IDocumentRepository
{
    public DocumentRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<DocumentMetadata?> GetDocumentByIdForUser(int documentId, int userId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        
        connection.Open();
        
        using var cmd = new NpgsqlCommand("get_document_by_id_for_user", connection);
    
        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("p_document_id", documentId);
        cmd.Parameters.AddWithValue("p_user_id", userId);

        cmd.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Integer)).Direction = System.Data.ParameterDirection.Output;
        cmd.Parameters.Add(new NpgsqlParameter("p_name", NpgsqlTypes.NpgsqlDbType.Varchar)).Direction = System.Data.ParameterDirection.Output;
        cmd.Parameters.Add(new NpgsqlParameter("p_description", NpgsqlTypes.NpgsqlDbType.Text)).Direction = System.Data.ParameterDirection.Output;
        cmd.Parameters.Add(new NpgsqlParameter("p_file_path", NpgsqlTypes.NpgsqlDbType.Text)).Direction = System.Data.ParameterDirection.Output;
        cmd.Parameters.Add(new NpgsqlParameter("p_created_by", NpgsqlTypes.NpgsqlDbType.Integer)).Direction = System.Data.ParameterDirection.Output;
        cmd.Parameters.Add(new NpgsqlParameter("p_created_at", NpgsqlTypes.NpgsqlDbType.Timestamp)).Direction = System.Data.ParameterDirection.Output;

        await cmd.ExecuteNonQueryAsync();

        if (cmd.Parameters["p_id"].Value is DBNull)
        {
            return null;
            
        }
        
        var id = (cmd.Parameters["p_id"]?.Value) is System.DBNull ? 0 : (int)cmd.Parameters["p_id"].Value!;
        var name = cmd.Parameters["p_name"].Value as string;
        var description = cmd.Parameters["p_description"].Value as string;
        var filePath = cmd.Parameters["p_file_path"].Value as string;
        var createdBy = (cmd.Parameters["p_created_by"]?.Value) is System.DBNull ? 0 : (int)cmd.Parameters["p_created_by"].Value!;
        var createdAt = (cmd.Parameters["p_created_at"]?.Value) is System.DBNull ? DateTime.MinValue : (DateTime)cmd.Parameters["p_created_at"].Value!;

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
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("p_name", document.Name);
        command.Parameters.AddWithValue("p_description", document.Description);
        command.Parameters.AddWithValue("p_file_path", document.FilePath);
        command.Parameters.AddWithValue("p_created_by", document.CreatedByUser);
        command.Parameters.AddWithValue("p_category", document.Category ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<DocumentMetadata>> GetDocumentsByUserId(int userId)
    {
        var documents = new List<DocumentMetadata>();

        using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync();

        using var command = new NpgsqlCommand("SELECT * FROM get_documents_by_user_id(@p_user_id)", connection);
        command.Parameters.AddWithValue("p_user_id", userId);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            if (reader.IsDBNull(reader.GetOrdinal("id"))) 
            {
                break;
            }

            var document = new DocumentMetadata
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name")
            };

            documents.Add(document);
        }

        return documents;
    }

}
