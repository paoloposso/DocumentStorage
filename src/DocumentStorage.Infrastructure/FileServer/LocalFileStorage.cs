using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentStorage.Document;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace DocumentStorage.Infrastructure.FileServer
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly IConfiguration _configuration;

        public LocalFileStorage(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<byte[]> ReadFileAsync(string filePath)
        {
            return File.ReadAllBytesAsync(filePath);
        }

        public async Task<string> StoreFileAsync(byte[] fileContent, string fileName)
        {
            var baseFilePath = _configuration.GetValue<string>("fileStorage:baseFilePath");

            if (string.IsNullOrEmpty(baseFilePath))
            {
                throw new InvalidOperationException("Base file path is not configured");
            }

            var fullPath = Path.Combine(baseFilePath, fileName);

            // Write the content to the file.
            await File.WriteAllBytesAsync(fullPath, fileContent);

            return fullPath;
        }
    }
}