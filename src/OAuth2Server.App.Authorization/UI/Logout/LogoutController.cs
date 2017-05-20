using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using OAuth2Server.App.Authorization.Configuration;
using Microsoft.Extensions.Options;
using OAuth2Server.Security.Managers;

namespace OAuth2Server.App.Authorization.UI.Logout
{
    public class LogoutController : Controller
    {
        private readonly IIdentityServerInteractionService interaction;
        private readonly IAuthUserManager authUserManager;
        private readonly IPersistedGrantService persistedGrantService;
        private readonly AuthAppSettings appSettings;

        public LogoutController(IIdentityServerInteractionService interaction, IPersistedGrantService persistedGrantService, IAuthUserManager authUserManager, IOptions<AuthAppSettings> appSettings)
        {
            this.interaction = interaction;
            this.persistedGrantService = persistedGrantService;
            this.authUserManager = authUserManager;
            this.appSettings = appSettings.Value;
        }

        [HttpGet("ui/logout", Name = "Logout")]
        public async Task<IActionResult> Logout()
        {
            var user = HttpContext.User.Identity.Name;
            var subjectId = string.Empty;

            try
            {
                subjectId = HttpContext.User.Identity.GetSubjectId();
            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(subjectId))
            {
                // delete authentication cookie
                await HttpContext.Authentication.SignOutAsync();
                await this.authUserManager.UserLogout();

                // set this so UI rendering sees an anonymous user
                HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

                await this.persistedGrantService.RemoveAllGrantsAsync(subjectId, "Client");
            }

            return Redirect(this.appSettings?.LogoutRedirectUri);
        }
    }
}
