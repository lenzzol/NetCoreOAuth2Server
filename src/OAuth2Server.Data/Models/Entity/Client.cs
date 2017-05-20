using System;

namespace OAuth2Server.Data.Models.Entity
{
    public class Client
    {
        public Guid ClientId { get; set; }
        public int StatusId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}
