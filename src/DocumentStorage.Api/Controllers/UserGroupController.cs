using Microsoft.AspNetCore.Mvc;
using DocumentStorage.User;
using DocumentStorage.Api.Model;

namespace DocumentStorage.Api.Controllers;

[ApiController]
[Route("[controller]")]
public partial class UserGroupController : ControllerBase
{
    private readonly ILogger<UserGroupController> _logger;
    private readonly IGroupService _service;

    public UserGroupController(ILogger<UserGroupController> logger, IGroupService service)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost(Name = "createGroup")]
    public async Task<ActionResult> Post([FromBody] AddGroupRequest request)
    {
        try
        {
            IList<string> validationResult = request.Validate();

            if (validationResult.Any())
            {
                return BadRequest(new { message = validationResult });
            }

            await _service.AddGroup(request.Name, request.Description);
            return Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to add group");
            return StatusCode(500, $"Failed to add group: {ex.Message}");
        }
    }

    [HttpGet(Name = "listGroups")]
    public async Task<ActionResult> Get()
    {
        try
        {
            var groups = await _service.ListGroups();
            return Ok(groups);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to list groups");
            return StatusCode(500, "Failed to list groups");
        }
    }

    [HttpGet("{groupId}")]
    public async Task<ActionResult> Get(int groupId)
    {
        try
        {
            var group = await _service.GetById(groupId);

            if (group is null)
            {
                return NotFound("Group not found");
            }
            return Ok(group);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting group by ID {GroupId}", groupId);
            return StatusCode(500);
        }
    }

    [HttpDelete("{groupId}", Name = "deleteGroupById")]
    public async Task<ActionResult> Delete(int groupId)
    {
        try
        {
            var result = await _service.DeleteGroupById(groupId);
            if(result.successful)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, result.message);
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete group with ID {groupId}");
            return StatusCode(500, $"Failed to delete group with ID {groupId}");
        }
    }

    [HttpPut("{groupId}")]
    public async Task<ActionResult> Put(int groupId, [FromBody] UpdateGroupRequest request)
    {
        try
        {
            IList<string> validationResult = request.Validate();

            if (validationResult.Any())
            {
                return BadRequest(new { message = validationResult });
            }

            var group = new UserGroup { Id = groupId, Name = request.Name, Description = request.Description };
            await _service.UpdateGroup(group);
            return Ok();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Failed to update group with ID {groupId}");
            return StatusCode(500, $"Failed to update group with ID {groupId}");
        }
    }
}
