namespace DocumentStorage.Document;

public interface IFileStorage
{
    Task<string> StoreFile(byte[] fileContent, string fileName);
    Task<byte[]> ReadFile(string filePath);
}
