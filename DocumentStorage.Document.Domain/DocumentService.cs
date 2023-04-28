using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Document.Domain
{
    public class DocumentService
    {
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