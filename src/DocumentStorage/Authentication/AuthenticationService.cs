using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DocumentStorage.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;
    private const string SECRET_KEY = "secret1234567890";

    public AuthenticationService(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    public async Task<(int, string)> Authenticate(string email, string password)
    {
        var (id, hashPassword) = await _authenticationRepository.GetUserAuthInfoByEmail(email);

        if (BCrypt.Net.BCrypt.Verify(password, hashPassword))
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: "documents-api",
                audience: "documents-api-client",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signingCredentials
            );

            var handler = new JwtSecurityTokenHandler();

            var tokenString = handler.WriteToken(token);

            return (id, tokenString);
        }

        throw new UnauthorizedAccessException("Invalid e-mail or password");
    }
}