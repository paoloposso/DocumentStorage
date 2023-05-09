namespace DocumentStorage.Document;

public interface IDocumentService
{
    Task<(string fileName, byte[] fileContent)> DownloadDocument(int documentId, int userId);
    Task UploadDocument(DocumentMetadata document, byte[] content);
    Task<DocumentMetadata> GetDocumentMetadata(int documentId, int userId);
    Task<IEnumerable<DocumentMetadata>> GetDocumentsByUserId(int userId);
}
