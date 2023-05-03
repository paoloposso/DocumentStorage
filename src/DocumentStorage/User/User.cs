using DocumentStorage.Core;

namespace DocumentStorage.User;

public struct User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool Active { get; set; }
    public List<UserGroup> Groups { get; set; }
    public Role Role { get; set; }
}
