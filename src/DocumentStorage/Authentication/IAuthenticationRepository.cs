namespace DocumentStorage.Authentication;

public interface IAuthenticationRepository
{
    Task<(string id, string hashedPassword)?> GetUserByEmail(string email);
}
