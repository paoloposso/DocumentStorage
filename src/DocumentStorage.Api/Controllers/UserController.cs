using Microsoft.AspNetCore.Mvc;
using DocumentStorage.User;
using DocumentStorage.Api.Model;

namespace DocumentStorage.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;

    public UserController(ILogger<UserController> logger, IUserService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost(Name = "createUser")]
    public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
    {
        try
        {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create User");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok();
    }

    [HttpPatch(Name = "updateUser")]
    public async Task<IActionResult> PatchUser([FromBody] UpdateUserRequest request)
    {
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
    public async Task<IActionResult> PatchUserGroup([FromBody] AddUserToGroupRequest request)
    {
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
