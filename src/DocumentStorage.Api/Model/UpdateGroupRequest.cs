using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DocumentStorage.Api.Model;

public class UpdateGroupRequest
{
    [JsonPropertyName("name")]
    [Required]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    public IList<string> Validate() 
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(Name)) 
        {
            result.Add("Name is required");
        }

        return result;
    }
}
