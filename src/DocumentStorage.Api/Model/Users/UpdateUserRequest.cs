using DocumentStorage.Core;

namespace DocumentStorage.Api.Model;
public class UpdateUserRequest
{
    public int Id { get; set; }
    public Role Role { get; set; }
    public bool Active { get; set; }

    public IList<string> Validate() 
    {
        var result = new List<string>();

        if (Id <= 0) 
        {
            result.Add("Id is required");
        }

        if (!Enum.IsDefined(typeof(Role), Role))
        {
            result.Add("Invalid role");
        }

        return result;
    }
}
