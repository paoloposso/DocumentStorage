namespace DocumentStorage.Document;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorage _fileStorage;

    public DocumentService(IDocumentRepository documentRepository, IFileStorage fileStorage)
    {
        _documentRepository = documentRepository;
        _fileStorage = fileStorage;
    }

    public async Task UploadDocument(DocumentMetadata document, byte[] content)
    {
        var filePath = await _fileStorage.StoreFileAsync(content, document.Name);

        document.FilePath = filePath;

        await _documentRepository.InsertDocumentMetadata(document);
    }

    public async Task<byte[]> DownloadDocument(int documentId, int userId)
    {
        var document = await _documentRepository.GetDocumentByIdForUser(documentId, userId);

        if (document is null || document?.Id <= 0
            || string.IsNullOrEmpty(document?.FilePath))
        {
            throw new ArgumentException("Document not found");
        }

        var content = await _fileStorage.ReadFileAsync(document?.FilePath!);

        return content;
    }

    public async Task<DocumentMetadata?> GetDocumentMetadate(int documentId, int userId)
    {
        var document = await _documentRepository.GetDocumentByIdForUser(documentId, userId);

        if (document is null || document?.Id <= 0
            || string.IsNullOrEmpty(document?.FilePath))
        {
            throw new ArgumentException("Document not found");
        }

        return document;
    }
}