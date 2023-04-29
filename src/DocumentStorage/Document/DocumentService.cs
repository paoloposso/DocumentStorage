using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Document
{
    public class DocumentService
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
            var filePath = await _fileStorage.StoreFile(content, document.Name);

            document.FilePath = filePath;

            await _documentRepository.InsertDocumentMetadata(document);
        }

        public async Task<byte[]> DownloadDocument(string documentId, string userId)
        {
            var document = await _documentRepository.GetDocumentMetadata(userId);

            return await _fileStorage.ReadFile(document.FilePath);
        }
    }
}