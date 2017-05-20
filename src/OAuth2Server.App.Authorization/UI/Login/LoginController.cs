using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OAuth2Server.Security.Managers;

namespace OAuth2Server.App.Authorization.UI.Login
{
    public class LoginController : Controller
    {
        private readonly ILogger log;
        private readonly IIdentityServerInteractionService interaction;
        private readonly IAuthUserManager authUserManager;

        public LoginController(IIdentityServerInteractionService interaction, ILogger<LoginController> logger, IAuthUserManager authUserManager)
        {
            this.interaction = interaction;
            this.log = logger;
            this.authUserManager = authUserManager;
        }

        [HttpGet("ui/login", Name = "Login")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = new LoginViewModel();

            if (returnUrl != null)
            {
                var context = await this.interaction.GetAuthorizationContextAsync(returnUrl);
                if (context != null)
                {
                    vm.Username = context.LoginHint;
                    vm.ReturnUrl = returnUrl;
                }
            }

            return View(vm);
        }

        [HttpPost("ui/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                model.AuthenticationResult = await this.authUserManager.Authenticate(model.Username, model.Password);

                if (model.AuthenticationResult == Security.AuthenticationResult.Success)
                {
                    if (await this.authUserManager.UserLogin(model.Username, model.Password, model.RememberLogin))
                    {
                        this.log.LogInformation(1, "User logged in.");

                        if (model.ReturnUrl != null && this.interaction.IsValidReturnUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        return Redirect("~/");
                    }

                    model.AuthenticationResult = Security.AuthenticationResult.Failed;
                }
            }

            var vm = new LoginViewModel(model);
            return View(vm);
        }
    }
}
