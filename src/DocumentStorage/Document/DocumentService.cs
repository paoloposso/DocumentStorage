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
        var filePath = await _fileStorage.StoreFileAsync(content, Guid.NewGuid() + document.Name);

        document.FilePath = filePath;
        document.PostedDate = DateTime.UtcNow;

        await _documentRepository.InsertDocumentMetadata(document);
    }

    public async Task<(string fileName, byte[] fileContent)> DownloadDocument(int documentId, int userId)
    {
        var document = await _documentRepository.GetDocumentByIdForUser(documentId, userId);

        if (document is null || document?.Id <= 0
            || string.IsNullOrEmpty(document?.FilePath))
        {
            throw new ArgumentException("Document not found");
        }

        var content = await _fileStorage.ReadFileAsync(document?.FilePath!);

        return (document!.Value.Name, content);
    }

    public async Task<DocumentMetadata?> GetDocumentMetadata(int documentId, int userId)
    {
        var document = await _documentRepository.GetDocumentByIdForUser(documentId, userId);

        if (document is null || document?.Id <= 0
            || string.IsNullOrEmpty(document?.FilePath))
        {
            throw new ArgumentException("Document not found");
        }

        return document;
    }

    public Task<IEnumerable<DocumentMetadata>> GetDocumentsByUserId(int userId)
    {
        return _documentRepository.GetDocumentsByUserId(userId);
    }
}