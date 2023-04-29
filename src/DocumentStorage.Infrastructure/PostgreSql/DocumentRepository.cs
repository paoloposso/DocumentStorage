using System.Data;
using Dapper;
using DocumentStorage.Document;
using Npgsql;

namespace DocumentStorage.Infrastructure.PostgreSql;

public class DocumentRepository : IDocumentRepository
{
    private readonly string _connectionString;

    public DocumentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<DocumentMetadata> GetDocumentMetadata(string id, string userId)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("p_document_id", id);
        parameters.Add("p_user_id", userId);

        using var gridReader = connection.QueryMultiple("document_get_by_id_with_access", parameters, commandType: CommandType.StoredProcedure);
        var doc = gridReader.ReadFirstOrDefault();

        if (doc is null)
        {
            throw new Exception("Document not found");
        }

        var result = new DocumentMetadata(
            postedDate: parameters.Get<DateTime>("p_created_at"),
            description: parameters.Get<string>("p_description"),
            name: parameters.Get<string>("p_name"),
            category: parameters.Get<string>("p_category"),
            filePath: parameters.Get<string>("p_file_path"),
            createdByUser: parameters.Get<string>("p_created_by")
        );

        return result;
    }

    public async Task InsertDocumentMetadata(DocumentMetadata document)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("p_name", document.Name);
        parameters.Add("p_description", document.Description);
        parameters.Add("p_file_path", document.FilePath);
        parameters.Add("p_created_by", document.CreatedByUser);

        connection.Execute("document_insert", parameters, commandType: CommandType.StoredProcedure);
    }
}
