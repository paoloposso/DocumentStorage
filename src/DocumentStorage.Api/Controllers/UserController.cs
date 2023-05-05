using Microsoft.AspNetCore.Mvc;
using DocumentStorage.User;
using DocumentStorage.Api.Model;
using Microsoft.AspNetCore.Authorization;
using DocumentStorage.Authentication;
using DocumentStorage.Core;

namespace DocumentStorage.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : BaseController
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;

    public UserController(
        ILogger<UserController> logger, 
        IUserService service, 
        IAuthenticationService authenticationService) : base(authenticationService)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost(Name = "createUser")]
    public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!Authorized(new Role[] { Role.Admin }))
            {
                return Unauthorized();
            }

            IList<string> validationResult = request.Validate();

            if (validationResult.Any())
            {
                return BadRequest(new { message = validationResult });
            }

            await _service.AddUser(new User.User {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Role = request.Role
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Failed to create User");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create User");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok();
    }

    
    [HttpGet("{userId}", Name = "getUser")]
    public async Task<ActionResult> Get(int userId)
    {
        try
        {
            if (!Authorized(new Role[] { Role.Admin }))
            {
                return Unauthorized();
            }

            var user = await _service.GetUser(userId);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get group");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPatch(Name = "updateUser")]
    public async Task<IActionResult> Patch([FromBody] UpdateUserRequest request)
    {
         if (!Authorized(new Role[] { Role.Admin }))
        {
            return Unauthorized();
        }

        var validationErrors = request.Validate();

        if (validationErrors.Any())
        {
            return BadRequest(new { message = validationErrors });
        }

        try
        {
            await _service.UpdateUser(request.Id, request.Role, request.Active);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok();
    }

    [HttpPatch("groups", Name = "addUserToGroups")]
    public async Task<IActionResult> Patch([FromBody] AddUserToGroupRequest request)
    {
        if (!Authorized(new Role[] { Role.Admin }))
        {
            return Unauthorized();
        }

        var validationErrors = request.Validate();

        if (validationErrors.Any())
        {
            return BadRequest(new { message = validationErrors });
        }

        try
        {
            await _service.AddUserToGroup(request.UserId, request.GroupIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to add User to Groups");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok();
    }
}
