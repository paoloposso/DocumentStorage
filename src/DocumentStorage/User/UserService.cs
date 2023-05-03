using DocumentStorage.Core;

namespace DocumentStorage.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task AddGroup(int id, string password)
    {
        throw new NotImplementedException();
    }

    public async Task AddUser(User user)
    {
        var password = user.Password;
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        user.Password = hashedPassword;
    
        await _userRepository.InsertUser(user);
    }

    public async Task UpdateUser(int id, Role role, bool active)
    {
        await _userRepository.UpdateUser(id, role, active);
    }

    public async Task AddUserToGroup(int userId, List<int> groups)
    {
        foreach (var group in groups)
        {
            await _userRepository.AddUserToGroup(userId, group);
        }
    }
}
