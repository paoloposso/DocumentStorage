using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Document;

public struct DocumentMetadata
{
    public string? Id { get; private set; }
    public DateTime PostedDate { get; private set; }
    public string Description { get; private set; }
    public string Name { get; private set; }
    public string Category { get; private set; }
    public string FilePath { get; set; }
    public IList<string> UsersCanAccess { get; set; }
    public IList<string> GroupsCanAccess { get; set; }
    public string CreatedByUser { get; set; }

    public void SetId(string id)
    {
        Id = id;
    }

    public DocumentMetadata(DateTime postedDate, string description, string name, string category, string filePath, string createdByUser)
    {
        PostedDate = postedDate;
        Description = description;
        Name = name;
        Category = category;
        FilePath = filePath;
        CreatedByUser = createdByUser;
        UsersCanAccess = new List<string>();
        GroupsCanAccess = new List<string>();
    }
}
