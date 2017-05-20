using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OAuth2Server.Data.Models.Domain;
using OAuth2Server.Security.Managers;
using System.Net;
using OAuth2Server.App.Authorization.Configuration;
using Microsoft.Extensions.Options;

namespace OAuth2Server.App.Authorization.UI.Account
{
    public class AccountController : Controller
    {
        private readonly IAuthUserManager authUserManager;
        private readonly ILogger log;
        private readonly AuthAppSettings appSettings;

        public AccountController(IAuthUserManager authUserManager, ILogger<AccountController> logger, IOptions<AuthAppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;

            if (this.appSettings == null)
            {
                throw new ArgumentNullException("App settings cannot be null");
            }

            this.authUserManager = authUserManager;
            this.log = logger;
        }

        [HttpGet("account/reset", Name = "ResetPassword")]
        public async Task<IActionResult> ResetPassword(Guid userId, string code, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var user = await this.authUserManager.GetUser(userId, cancellationToken);
                var token = WebUtility.UrlDecode(code);

                this.log.LogDebug($"Decoded token for userid: {user.UserId} is {token}");

                if (user != null && !string.IsNullOrWhiteSpace(code))
                {
                    return this.View(new ResetPasswordViewModel
                    {
                        Email = user.Email,
                        Code = token
                    });
                }
            }
            catch (Exception ex)
            {
                this.log.LogError($"Error occurred retrieving user info for password reset. UserId: {userId}", ex);
            }

            return this.RedirectToAction("Error", "Home"); //TODO: need to redirect to error page
        }

        [HttpPost("account/reset")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            UserDto user = null;

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            try
            {
                user = await this.authUserManager.GetUser(model.Email, cancellationToken);
                user.Password = model.Password;

                var updateResult = await this.authUserManager.UpdateUser(user, cancellationToken, model.Code);

                if (updateResult)
                {
                    return this.RedirectToAction("Logout", "Logout");
                }
            }
            catch (Exception ex)
            {
                if (user == null)
                {
                    this.log.LogInformation($"User requested password reset but was not found in system. Email was {model.Email}");
                    this.log.LogError("Error retrieving user", ex);
                    //Don't reveal that the user does not exist
                }
            }

            return this.RedirectToAction("Error", "Home"); //TODO: need to redirect to error page
        }

        [HttpGet("account/resetrequest", Name = "ResetRequest")]
        public async Task<IActionResult> ResetRequest()
        {
            return this.View(new ResetRequestViewModel());
        }

        [HttpPost("account/resetrequest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetRequest(ResetRequestViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(model);
                }

                var user = await this.authUserManager.GetUser(model.ResetEmail, cancellationToken);

                if (user?.UserId != null)
                {
                    await this.authUserManager.SendEmailReset(user.UserId.Value, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                this.log.LogError(new EventId(2,"ResetRequest"), ex, $"Error occurred attempting to send reset password request for email: {model.ResetEmail}");
            }

            return this.View("ResetRequestConfirm");
        }

        [HttpGet("account/confirm", Name = "ConfirmAccount")]
        public async Task<IActionResult> ConfirmAccount(Guid userId, string code, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var user = await this.authUserManager.GetUser(userId, cancellationToken);

                if (user != null && user.UserId.HasValue && !string.IsNullOrWhiteSpace(code))
                {
                    var token = WebUtility.UrlDecode(code);
                    this.log.LogDebug($"Decoded token for userid: {user.UserId} is {token}");

                    var vm = new ConfirmAccountViewModel
                    {
                        AlreadyConfirmed = await this.authUserManager.IsAccountConfirmed(user.UserId.Value),
                        Code = token
                    };

                    return this.View(vm);
                }
            }
            catch (Exception ex)
            {
                this.log.LogError($"User requested confirm account but an error occurred.  UserId is {userId}", ex);
            }

            return this.RedirectToAction("Error", "Home"); //TODO: need to redirect to error page
        }

        [HttpPost("account/confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAccount(ConfirmAccountViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            UserDto user = null;

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            try
            {
                user = await this.authUserManager.GetUser(model.Email, cancellationToken);
                user.Password = model.Password;
                //user.StatusMasterKey = ?; TODO
                

                var updateResult = await this.authUserManager.UpdateUser(user, cancellationToken, model.Code);

                if (updateResult)
                {
                    await this.authUserManager.ConfirmAccount(user.UserId.Value, cancellationToken);
                }

                /* any existing user will be logged out then user 
                 * will be redirected to client home login screen*/
                return this.RedirectToAction("Logout", "Logout"); 

            }
            catch (Exception ex)
            {
                if (user == null)
                {
                    this.log.LogInformation($"User requested password reset but was not found in system. Email was {model.Email}");
                    this.log.LogError("Error retrieving user", ex);
                    //Don't reveal that the user does not exist
                }
            }

            return this.RedirectToAction("Error", "Home"); //TODO: need to redirect to error page
        }
    }
}


