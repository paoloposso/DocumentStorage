namespace DocumentStorage.Document;

public interface IDocumentService
{
    Task<(DocumentMetadata metadata, byte[] content)> DownloadDocument(int documentId, int userId);
    Task UploadDocument(DocumentMetadata document, byte[] content);
}
