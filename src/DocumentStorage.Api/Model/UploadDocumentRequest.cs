namespace DocumentStorage.Api.Model
{
    public class UploadDocumentRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public IFormFile? File { get; set; }

        public IEnumerable<string> Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return "Name is required";
            }

            if (string.IsNullOrWhiteSpace(Category))
            {
                yield return "Category is required";
            }

            if (File == null || File.Length == 0)
            {
                yield return "File is required";
            }
        }
    }
}
