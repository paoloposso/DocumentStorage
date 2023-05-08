using System.Text;
using DocumentStorage.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DocumentStorage.Infrastructure.Jwt;

public class AuthenticationKeyRepository : IAuthenticationKeyRepository
{
    private readonly IConfiguration _configuration;

    public AuthenticationKeyRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SigningCredentials GetSecrectKey()
    {
        var secretKeyString = _configuration.GetValue<string>("jwt:key") ?? string.Empty;
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));

        return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    }
}
