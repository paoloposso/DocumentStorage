using Microsoft.AspNetCore.Mvc;
using DocumentStorage.Authentication;
using DocumentStorage.Api.Model;
using System.Security.Cryptography;
using System.Text;

namespace DocumentStorage.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : BaseController
{
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ILogger<AuthenticationController> logger, 
        IAuthenticationService authenticationService) : base(authenticationService)
    {
        _logger = logger;
    }

    [HttpPost(Name = "authenticateUser")]
    public async Task<IActionResult> Post([FromBody] AuthenticationRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email and Password are Required"});
        }

        try
        {
            var (id, token) =  await _authenticationService.Authenticate(request.Email, request.Password);

            return Ok(new AuthenticationResponse(token, id));
        }
        catch (UnauthorizedAccessException ex) 
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
