namespace DocumentStorage.Document.Domain
{
    public enum Role {
        Regular,
        Manager,
        Admin
    }

    public struct UserGroup
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public UserGroup(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}