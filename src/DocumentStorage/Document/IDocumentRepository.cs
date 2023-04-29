namespace DocumentStorage.Document;

public interface IDocumentRepository
{
    Task InsertDocumentMetadata(DocumentMetadata document);
    Task<DocumentMetadata> GetDocumentMetadata(string id, string userId);
}