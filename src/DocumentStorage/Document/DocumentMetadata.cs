namespace DocumentStorage.Document;

public struct DocumentMetadata
{
    public int Id { get; set; }
    public DateTime PostedDate { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }
    public string? Category { get; set; }
    public int CreatedByUser { get; set; }
}
