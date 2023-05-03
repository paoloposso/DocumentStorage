namespace DocumentStorage.User;

public interface IGroupRepository
{
    Task AddGroup(string name, string description);
    Task<IEnumerable<User>> ListGroupMembers(int groupId);
    Task<IEnumerable<UserGroup>> ListGroups();
    Task UpdateGroupName(int id, string name);
    Task<(bool successful, string? message)> DeleteGroupById(int id);
}
