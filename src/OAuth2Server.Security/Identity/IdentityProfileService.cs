using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using OAuth2Server.Data.Models.Entity;
using OAuth2Server.Security.Managers;
using static IdentityServer4.IdentityServerConstants;

namespace OAuth2Server.Security.Identity
{
    public class IdentityProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<AuthUser> claimsFactory;
        private readonly IAuthUserManager authUserManager;

        public IdentityProfileService(IAuthUserManager authUserManager , IUserClaimsPrincipalFactory<AuthUser> claimsFactory)
        {
            this.authUserManager = authUserManager;
            this.claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await this.authUserManager.FindById(sub);
            var userDto = await this.authUserManager.GetUser(user.UserId);
            var principal = await this.claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();

            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                claims.Add(new Claim(JwtClaimTypes.Name, user.UserName));
            }

            if (!string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(new Claim(JwtClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"));
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
                //claims.Add(new Claim(StandardScopes.Email.Name, user.Email));
                claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString(), System.Security.Claims.ClaimValueTypes.Boolean));
            }

            claims.Add(new Claim(JwtClaimTypes.Scope, "accountService")); // hardcoded account management service scope
            claims.Add(new Claim(JwtClaimTypes.Scope, "accounts"));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await this.authUserManager.FindById(sub);
            context.IsActive = user != null;
        }
    }
}
