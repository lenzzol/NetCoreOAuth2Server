using System.ComponentModel.DataAnnotations;
using OAuth2Server.Security;

namespace OAuth2Server.App.Authorization.UI.Login
{
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
        public AuthenticationResult AuthenticationResult { get; set; }
    }
}