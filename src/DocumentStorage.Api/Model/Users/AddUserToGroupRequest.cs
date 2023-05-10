using System.Text.Json.Serialization;

namespace DocumentStorage.Api.Model;

public class AddUserToGroupRequest
{
    [JsonPropertyName("groupId")]
    public required List<int> GroupIds { get; set; }

    [JsonPropertyName("userId")]
    public required int UserId { get; set; }

    public IList<string> Validate() 
    {
        var result = new List<string>();

        if (UserId <= 0) 
        {
            result.Add("UserId is required");
        }
        if (GroupIds is null || !GroupIds.Any()) 
        {
            result.Add("GroupIds is required");
        }

        return result;
    }
}
