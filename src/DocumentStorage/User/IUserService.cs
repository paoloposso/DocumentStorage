using DocumentStorage.Core;

namespace DocumentStorage.User;

public interface IUserService
{
    public Task<int> AddUser(User user);
    public Task UpdateUser(int id, Role role, bool active);
    public Task AddUserToGroup(int userId, List<int> groupIds);
}
