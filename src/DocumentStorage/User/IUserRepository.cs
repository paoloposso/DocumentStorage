using DocumentStorage.Core;

namespace DocumentStorage.User;

public interface IUserRepository
{
    Task UpdateUser(int id, Role role, bool active);
    Task<int> InsertUser(User user);
    Task<IEnumerable<User>> ListUsers();
    Task AddUserToGroup(int userId, int groupId);
    Task<User?> GetUserById(int id);
}