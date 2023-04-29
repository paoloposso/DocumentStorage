using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Document
{
    public interface IFileStorage
    {
        Task<string> StoreFile(byte[] fileContent, string fileName);
        Task<byte[]> ReadFile(string filePath);
    }
}