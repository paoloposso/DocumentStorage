namespace DocumentStorage.Authentication;

public class AuthenticationResponse
{
    public string Token { get; private set; }    
    public string UserId { get; private set; }

    public AuthenticationResponse(string token, string userId)
    {
        Token = token;
        UserId = userId;
    }
}
