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

    public Task<UserGroup?> GetById(int id)
    {
        return _repository.GetGroupById(id);
    }
}