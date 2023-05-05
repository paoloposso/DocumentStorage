namespace DocumentStorage.Document;

public interface IDocumentRepository
{
    Task<DocumentMetadata?> GetDocumentByIdForUser(int documentId, int userId);
    Task InsertDocumentMetadata(DocumentMetadata document);
}