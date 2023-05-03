using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DocumentStorage.Api.Model;

public class AddGroupRequest
{
    [JsonPropertyName("name")]
    [Required]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    [Required]
    public required string Description { get; set; }

    public IList<string> Validate() 
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(Name)) 
        {
            result.Add("Name is required");
        }
        if (string.IsNullOrEmpty(Description)) 
        {
            result.Add("Description is required");
        }

        return result;
    }
}

