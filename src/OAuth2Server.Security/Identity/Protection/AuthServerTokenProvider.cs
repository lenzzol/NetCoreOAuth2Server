using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace OAuth2Server.Security.Identity.Protection
{
    public class AuthServerTokenProvider<TUser> : DataProtectorTokenProvider<TUser>, IUserTwoFactorTokenProvider<TUser> where TUser : class
    {
        public AuthServerTokenProvider(IAuthServerDataProtectionProvider dataProtectProvider, IOptions<DataProtectionTokenProviderOptions> protectOptions) : base(dataProtectProvider, protectOptions)
        {
        }
    }
}
