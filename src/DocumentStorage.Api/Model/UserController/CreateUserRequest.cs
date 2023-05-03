using System.Text.Json.Serialization;
using DocumentStorage.Core;
using DocumentStorage.User;

namespace DocumentStorage.Api.Model.UserController
{
    public class CreateUserRequest
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("password")]
        public required string Password { get; set; }
        
        [JsonPropertyName("role")]
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

            return result;
        }
    }
}