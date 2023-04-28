using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Document.Domain
{
    public struct Document
    {
        public string? Id { get; private set; }
        public DateTime PostedDate { get; private set; }
        public string Description { get; private set; }
        public string Name { get; private set; }
        public string Category { get; private set; }
        public IList<string> UsersCanAccess { get; set; }
        public IList<string> GroupsCanAccess { get; set; }

        public void SetId(string id) 
        {
            Id = id;
        }

        public Document(DateTime postedDate, string description, string name, string category)
        {
            PostedDate = postedDate;
            Description = description;
            Name = name;
            Category = category;
            UsersCanAccess = new List<string>();
            GroupsCanAccess = new List<string>();
        }
    }
}