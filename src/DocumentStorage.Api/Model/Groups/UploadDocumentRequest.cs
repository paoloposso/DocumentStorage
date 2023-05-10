using System.Text.Json.Serialization;

namespace DocumentStorage.Api.Model
{
    public class UploadDocumentRequest
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("file")]
        public IFormFile? File { get; set; }

        public IEnumerable<string> Validate()
        {
            if (File == null || File.Length == 0)
            {
                yield return "File is required";
            }
        }
    }
}
