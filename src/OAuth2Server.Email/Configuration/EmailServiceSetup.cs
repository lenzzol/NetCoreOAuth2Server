using System.Collections.Generic;

namespace OAuth2Server.Email.Configuration
{
    public class EmailServiceSetup
    {
        public List<EmailProvider> Providers { get; set; }
        public List<EmailTemplate> EmailTemplates { get; set; }
    }
}
