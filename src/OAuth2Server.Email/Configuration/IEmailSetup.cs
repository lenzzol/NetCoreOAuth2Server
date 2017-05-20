using OAuth2Server.Email.Model;

namespace OAuth2Server.Email.Configuration
{
    public interface IEmailSetup
    {
        string Name { get; set; }
        string FromEmailAddress { get; set; }
        string FromFriendlyName { get; set; }
        string Host { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        int? Port { get; set; }
        bool UseDefaultCredentials { get; set; }
        EmailAddress DefaultSender { get; set; }
        bool Debug { get; set; }
    }
}