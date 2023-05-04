namespace DocumentStorage.Authentication;

public interface IAuthenticationRepository
{
    Task<(int id, string? hash, int role)> GetUserAuthInfoByEmail(string email);
}
