using DocumentStorage.Core;

namespace DocumentStorage.Authentication;

public interface IAuthenticationService
{
    Task<(int id, string token)> Authenticate(string email, string password);
    (Role role, string? email, int id) GetClaims(string token);
}
