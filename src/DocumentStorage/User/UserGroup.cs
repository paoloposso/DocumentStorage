namespace DocumentStorage.User
{
    public struct UserGroup
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public UserGroup(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}