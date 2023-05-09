using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DocumentStorage.Core;
using DocumentStorage.User;

namespace DocumentStorage.Api.Model;

public class CreateUserRequest
{
    [JsonPropertyName("name")]
    [Required]
    public required string Name { get; set; }

    [JsonPropertyName("email")]
    [Required]
    public required string Email { get; set; }

    [JsonPropertyName("password")]
    [Required]
    public required string Password { get; set; }
    
    [JsonPropertyName("role")]
    [Required]
    public Role Role { get; set; }

    public IList<string> Validate() 
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(Name)) 
        {
            result.Add("Name is required");
        }
        if (string.IsNullOrEmpty(Email)) 
        {
            result.Add("Email is required");
        }
        if (string.IsNullOrEmpty(Password)) 
        {
            result.Add("Password is required");
        }
        if (!Enum.IsDefined(typeof(Role), Role)) 
        {
            result.Add("Invalid role");
        }

        return result;
    }
}