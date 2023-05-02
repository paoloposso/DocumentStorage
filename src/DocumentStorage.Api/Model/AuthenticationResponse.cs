namespace DocumentStorage.Authentication;

public class AuthenticationResponse
{
    public string Token { get; private set; }    
    public int UserId { get; private set; }

    public AuthenticationResponse(string token, int userId)
    {
        Token = token;
        UserId = userId;
    }
}
