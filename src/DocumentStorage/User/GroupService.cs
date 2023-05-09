namespace DocumentStorage.User;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _repository;

    public GroupService(IGroupRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<UserGroup>> ListGroups()
    {
        return _repository.ListGroups();
    }

    public Task AddGroup(string name, string description)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Name is required");
        }

        return _repository.AddGroup(name, description);
    }

    public Task UpdateGroup(UserGroup group)
    {
        return _repository.Update(group);
    }

    public Task<(bool successful, string? message)> DeleteGroupById(int id)
    {
        return _repository.DeleteGroupById(id);
    }

    public async Task<UserGroup?> GetById(int id)
    {
        var result = await _repository.GetGroupById(id);
        if (result is null) 
        {
            throw new ArgumentException($"Group with ID {id} not found");
        }
        return result;
    }

    public Task<IEnumerable<User>> GetUsersInGroup(int id)
    {
        return _repository.GetUsersInGroup(id);
    }
}