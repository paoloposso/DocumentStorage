using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DocumentStorage.Core;
using Microsoft.Extensions.Configuration;

namespace DocumentStorage.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;
    private readonly IAuthenticationKeyRepository _authenticationKeyRepository;

    public AuthenticationService(IAuthenticationRepository authenticationRepository, 
        IAuthenticationKeyRepository authenticationKeyRepository)
    {
        _authenticationKeyRepository = authenticationKeyRepository;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<(int id, string token)> Authenticate(string email, string password)
    {
        var (id, hashPassword, role) = await _authenticationRepository.GetUserAuthInfoByEmail(email);

        if (id <= 0 || string.IsNullOrEmpty(hashPassword) || role < 0)
        {
            throw new UnauthorizedAccessException("Invalid e-mail or password");
        }

        if (BCrypt.Net.BCrypt.Verify(password, hashPassword))
        {
            Core.Role userRole = (Core.Role)role;

            var signingCredentials = _authenticationKeyRepository.GetSecrectKey();

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, userRole.ToString()),
                new Claim("id", id.ToString()) // Custom claim
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

    public (Role role, string? email, int id) GetClaims(string token)
    {
        var jwtToken = new JwtSecurityToken(token);

        var claims = jwtToken.Claims?.Where(c => c.Type != null);

        if (claims is null || !claims.Any())
        {
            throw new UnauthorizedAccessException("Invalid token");
        }

        var role = claims!.FirstOrDefault(c => c.Type!.Equals(ClaimTypes.Role.ToString()))?.Value ?? "string";
        string? email = claims!.FirstOrDefault(c => c.Type!.Equals(ClaimTypes.Email))?.Value;
        string? id = claims!.FirstOrDefault(c => c.Type!.Equals("id"))?.Value;

        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(id))
        {
            throw new UnauthorizedAccessException("Invalid token");
        }

        if (Enum.TryParse<Role>(role, out Role result))
        {
            return (result, email, Convert.ToInt32(id));
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid token");
        }
    }
}