using Microsoft.AspNetCore.DataProtection;

namespace OAuth2Server.Security.Identity.Protection
{
    public class AuthServerDataProtectionProvider : IAuthServerDataProtectionProvider
    {
        static IAuthServerDataProtector dataProtector;

        public IDataProtector CreateProtector(string purpose)
        {
            dataProtector = dataProtector ?? new AuthServerDataProtector();
            return dataProtector;
        }
    }
}
