using System.Text.Json.Serialization;

namespace DocumentStorage.Api.Model;

public class AuthenticationResponse
{
    [JsonPropertyName("token")]
    public string Token { get; private set; }  
    
    [JsonPropertyName("userId")]
    public int UserId { get; private set; }

    public AuthenticationResponse(string token, int userId)
    {
        Token = token;
        UserId = userId;
    }
}
