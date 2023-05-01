using Microsoft.AspNetCore.Mvc;
using DocumentStorage.Authentication;
using DocumentStorage.Api.Model;

namespace DocumentStorage.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly AuthenticationService _authenticationService;

    public AuthenticationController(ILogger<UserController> logger, AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    [HttpPost(Name = "authenticateUser")]
    public async Task<IActionResult> Post([FromBody] AuthenticationRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email and Password are Required"});
        }

        var (token, id) =  await _authenticationService.Authenticate(request.Email, request.Password);

        return Ok(new AuthenticationResponse(token, id));
    }
}
