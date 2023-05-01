using Microsoft.AspNetCore.Mvc;
using DocumentStorage.User;
using DocumentStorage.Api.Model.UserController;

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

    [HttpPost(Name = "addUser")]
    public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
    {
        try
        {
            await _service.AddUser(new User.User {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Role = request.Role
            });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }

        return Ok();
    }

    [HttpPatch(Name = "UpdateUserRole")]
    public async Task<IActionResult> PatchUpdateUserRole([FromBody] UpdateRole request)
    {
        try
        {
            await _service.UpdateRole(request.Id, request.Role);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }

        return Ok();
    }
}
