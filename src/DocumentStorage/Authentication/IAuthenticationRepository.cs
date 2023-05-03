namespace DocumentStorage.Authentication;

public interface IAuthenticationRepository
{
    Task<(int, string?)> GetUserAuthInfoByEmail(string email);
}
