using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentStorage.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DocumentStorage.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;
    private readonly string _secretKey;
    private readonly IConfiguration _configuration;

    public AuthenticationService(
        IAuthenticationRepository authenticationRepository, 
        IConfiguration configuration)
    {
        _configuration = configuration;
        _authenticationRepository = authenticationRepository;
        _secretKey = _configuration.GetValue<string>("jwt:key") ?? string.Empty;
    }

    public async Task<(int id, string token)> Authenticate(string email, string password)
    {
        var (id, hashPassword, role) = await _authenticationRepository.GetUserAuthInfoByEmail(email);

        if (BCrypt.Net.BCrypt.Verify(password, hashPassword))
        {
            Core.Role userRole = (Core.Role)role;

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

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