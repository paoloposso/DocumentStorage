namespace DocumentStorage.Authentication;

public interface IAuthenticationRepository
{
    Task<(int id, string? hash)> GetUserAuthInfoByEmail(string email);
}
