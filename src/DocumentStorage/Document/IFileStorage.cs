namespace DocumentStorage.Document;

public interface IFileStorage
{
    Task<string> StoreFileAsync(byte[] fileContent, string fileName);
    Task<byte[]> ReadFileAsync(string filePath);
}
