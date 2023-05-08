using Microsoft.IdentityModel.Tokens;

namespace DocumentStorage.Authentication
{
    public interface IAuthenticationKeyRepository
    {
        SigningCredentials GetSecrectKey();
    }
}