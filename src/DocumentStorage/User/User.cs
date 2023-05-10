using DocumentStorage.Core;

namespace DocumentStorage.User;

public struct User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool Active { get; set; }
    public Role Role { get; set; }

    public IEnumerable<string> Validate() 
    {
        if (string.IsNullOrEmpty(Name))
        {
            yield return "Name is required";
        }
        if (string.IsNullOrEmpty(Email))
        {
            yield return "Email is required";
        }
        if (string.IsNullOrEmpty(Password))
        {
            yield return "Password is required";
        }
        if (!Enum.IsDefined(typeof(Role), Role))
        {
            yield return "Invalid role";
        }
    }
}
