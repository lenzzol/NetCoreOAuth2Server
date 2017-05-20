using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth2Server.Email.Configuration
{
    public class EmailProvider
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } 
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Debug { get; set; }
    }
}
