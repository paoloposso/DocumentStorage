using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Document;

namespace DocumentStorage.Infrastructure.FileServer
{
    public class LocalFileStorage : IFileStorage
    {
        public Task<byte[]> ReadFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<string> StoreFileAsync(byte[] fileContent, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}