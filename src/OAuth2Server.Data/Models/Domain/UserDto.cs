using System;
using System.Collections.Generic;

namespace OAuth2Server.Data.Models.Domain
{
    public class UserDto
    {
        public Guid UpdatedByUserId { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; } 
        public Guid ClientId { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}
