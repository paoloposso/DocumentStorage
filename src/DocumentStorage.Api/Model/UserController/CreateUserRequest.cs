using System.Text.Json.Serialization;
using DocumentStorage.Core;
using DocumentStorage.User;

namespace DocumentStorage.Api.Model.UserController
{
    public class CreateUserRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
        
        [JsonPropertyName("role")]
        public Role Role { get; set; }
    }
}