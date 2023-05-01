using DocumentStorage.Core;
using DocumentStorage.User;

namespace DocumentStorage.Api.Model.UserController
{
    public class UpdateRole
    {
        public int Id { get; set; }
        public Role Role { get; set; }
    }
}