using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Document;

public interface IDocumentRepository
{
    Task InsertDocumentMetadata(DocumentMetadata document);
    Task<DocumentMetadata> GetDocumentMetadata(string id, string userId);
}