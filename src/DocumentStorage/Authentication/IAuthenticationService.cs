namespace DocumentStorage.Authentication;

public interface IAuthenticationService
{
    Task<(int id, string token)> Authenticate(string email, string password);
}
