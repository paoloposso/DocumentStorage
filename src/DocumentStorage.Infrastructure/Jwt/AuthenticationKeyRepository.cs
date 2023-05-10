using System.Security.Cryptography;
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
        var secret = Convert.FromBase64String(_configuration?.GetValue<string>("jwt:secretKey"));
        var symmetricKey = Convert.FromBase64String(_configuration?.GetValue<string>("jwt:encryptionKey"));

        using var aes = Aes.Create();

        aes.Key = symmetricKey;
        aes.IV = secret.Take(16).ToArray();

        byte[] ciphertext = secret.Skip(16).ToArray();

        using ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] decryptedKey = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        string jwtSecretKey = Encoding.UTF8.GetString(decryptedKey);

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));

        return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    }
}
