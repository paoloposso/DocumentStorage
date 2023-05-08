namespace DocumentStorage.Document;

public interface IDocumentService
{
    Task<byte[]> DownloadDocument(int documentId, int userId);
    Task UploadDocument(DocumentMetadata document, byte[] content);
    Task<DocumentMetadata?> GetDocumentMetadate(int documentId, int userId);
}
