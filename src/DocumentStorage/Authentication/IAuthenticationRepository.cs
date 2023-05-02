namespace DocumentStorage.Authentication;

public interface IAuthenticationRepository
{
    Task<(int, string?, string?)> GetUserAuthInfoByEmail(string email);
}
