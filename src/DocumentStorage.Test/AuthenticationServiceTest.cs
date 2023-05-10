using System.Text;
using DocumentStorage.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace DocumentStorage.Test;

public class AuthenticationServiceTest
{
    AuthenticationService? _service;

    string VALID_PASSWORD = "12345678abcd123";
    string SECRET_KEY = "KEY123456&yyJKUiii89077_sdfg";

    [SetUp]
    public void Setup()
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(VALID_PASSWORD, salt);

        var repository = new Mock<IAuthenticationRepository>();
        var keyRepository = new Mock<IAuthenticationKeyRepository>();

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));

        var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        keyRepository
            .Setup(p => p.GetSecrectKey())
            .Returns(creds);
        
        repository
            .Setup(p => p.GetUserAuthInfoByEmail(It.Is<string>(p => p.Equals("test@test.com"))))
            .ReturnsAsync(() => (1, hashedPassword, 2));

            _service = new AuthenticationService(repository.Object, keyRepository.Object);
    }

    [Test]
    public async Task ShouldAuthenticateCorrectly()
    {
        var (id, token) = await _service!.Authenticate("test@test.com", VALID_PASSWORD);

        Assert.IsTrue(id > 0);
        Assert.IsTrue(token != null && token.Length > 0);
    }

    [Test]
    public async Task ShouldFailAuthentication()
    {
        try 
        {
            await _service!.Authenticate("test@test.com", "RANDOM_INVALID_PASSWORD");

            Assert.Fail("Authentication should fail for invalid password");
        }
        catch (UnauthorizedAccessException)
        {
            Assert.Pass();
        }
    }
}