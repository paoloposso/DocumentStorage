using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Document;

public interface IDocumentRepository
{
    Task InsertDocumentMetadata(Document document);
    Task<Document> GetDocumentMetadata(string id);
}