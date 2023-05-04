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

    public async Task<int> AddUser(User user)
    {
        var validationErrors = user.Validate();

        if (validationErrors.Any())
        {
            throw new ArgumentException(string.Join(",", validationErrors));
        }

        var password = user.Password;
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        user.Password = hashedPassword;
    
        return await _userRepository.InsertUser(user);
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

    public async Task<User?> GetUser(int id)
    {
        var result = await _userRepository.GetUserById(id);

        if (result is null) 
        {
            throw new ArgumentException($"User with id {id} not found");
        }

        return result;
    }
}
