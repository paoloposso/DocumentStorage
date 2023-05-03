namespace DocumentStorage.Authentication;

public interface IAuthenticationService
{
    Task<(int, string)> Authenticate(string email, string password);
}
