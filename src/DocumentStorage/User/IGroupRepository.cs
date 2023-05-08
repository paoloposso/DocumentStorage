namespace DocumentStorage.User;

public interface IGroupRepository
{
    Task AddGroup(string name, string description);
    Task<IEnumerable<User>> ListGroupMembers(int groupId);
    Task<IEnumerable<UserGroup>> ListGroups();
    Task Update(UserGroup group);
    Task<(bool successful, string? message)> DeleteGroupById(int id);
    Task<UserGroup?> GetGroupById(int groupId);
    Task<IEnumerable<User>> GetUsersInGroup(int groupId);
}
