using DocumentStorage.Core;

namespace DocumentStorage.User;

public class UserService : IUserService
{
    public UserService()
    {
        
    }

    public Task AddGroup(int id, string password)
    {
        throw new NotImplementedException();
    }

    public Task AddUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRole(int id, Role role)
    {
        throw new NotImplementedException();
    }
}
