namespace DocumentStorage.Core;

public struct User
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public List<UserGroup> Groups { get; private set; }
    public Role Role { get; set; }

    public void AddGroup(UserGroup group)
    {
        Groups.Add(group);
    }

    public User(string id, string name, Role role)
    {
        Id = id;
        Name = name;
        Role = role;
        Groups = new List<UserGroup>();
    }
}
