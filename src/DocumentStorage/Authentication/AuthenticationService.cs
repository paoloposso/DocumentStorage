namespace DocumentStorage.Authentication;

public class AuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;
    
    public AuthenticationService(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    public Task<(string, string)> Authenticate(string username, string password)
    {
        throw new NotImplementedException();
    }
}