using System;

namespace OAuth2Server.Data.Models.Entity
{
    public class Status
    {
        public int StatusId { get; set; }
        public Guid MasterKey { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
