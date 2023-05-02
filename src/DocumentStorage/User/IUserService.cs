using DocumentStorage.Core;

namespace DocumentStorage.User;

public interface IUserService
{
    public Task AddUser(User user);
    public Task UpdateRole(int id, Role role);
    public Task AddGroup(int id, string password);
}
