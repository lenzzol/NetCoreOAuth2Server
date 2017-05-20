using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth2Server.App.Authorization.Configuration
{
    public class AuthAppSettings
    {
        public string RedirectUri { get; set; }
        public string LogoutRedirectUri { get; set; }
        public string ClientMobilUri { get; set; }
    }
}
