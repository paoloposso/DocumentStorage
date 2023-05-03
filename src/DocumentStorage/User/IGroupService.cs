namespace DocumentStorage.User;

public interface IGroupService
{
    Task AddGroup(string name, string description);
    Task<(bool successful, string? message)> DeleteGroupById(int id);
    Task<IEnumerable<UserGroup>> ListGroups();
    Task<UserGroup?> GetById(int id);
    Task UpdateGroup(UserGroup group);
}
