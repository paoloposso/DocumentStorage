namespace DocumentStorage.User;

public interface IUserRepository
{
    Task UpdateUserRole(string email, string role);
    Task<int> InsertUser(User user);
    Task AddUserToGroup(int userId, int groupId);
}