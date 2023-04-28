using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Document
{
    public class DocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public Task UploadDocument(Document document)
        {
            throw new NotImplementedException();
        }

        public Task<Document> DownloadDocument(string documentId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}