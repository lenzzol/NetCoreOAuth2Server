using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace OAuth2Server.App.Authorization.UI.Consent
{
    public class ConsentController : Controller
    {
        private readonly ILogger<ConsentController> logger;
        private readonly IClientStore clientStore;
        private readonly IIdentityServerInteractionService interaction;
        private readonly IResourceStore resourceStore;

        public ConsentController(ILogger<ConsentController> logger, IIdentityServerInteractionService interaction, IClientStore clientStore, IResourceStore resourceStore)
        {
            this.logger = logger;
            this.interaction = interaction;
            this.clientStore = clientStore;
            this.resourceStore = resourceStore;
        }

        [HttpGet("ui/consent", Name = "Consent")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return View("Index", vm);
            }

            return View("Error");
        }

        [HttpPost("ui/consent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string button, ConsentInputModel model)
        {
            var request = await interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            ConsentResponse response = null;

            if (button == "no")
            {
                response = ConsentResponse.Denied;
            }
            else if (button == "yes" && model != null)
            {
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    response = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesConsented = model.ScopesConsented
                    };
                }
                else
                {
                    ModelState.AddModelError("", "You must pick at least one permission.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Selection");
            }

            if (response != null)
            {
                await interaction.GrantConsentAsync(request, response);
                return Redirect(model.ReturnUrl);
            }

            var vm = await BuildViewModelAsync(model.ReturnUrl, model);
            if (vm != null)
            {
                return View("Index", vm);
            }

            return View("Error");
        }

        async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null)
        {
            if (returnUrl != null)
            {
                var request = await interaction.GetAuthorizationContextAsync(returnUrl);
                if (request != null)
                {
                    var client = await clientStore.FindClientByIdAsync(request.ClientId);
                    if (client != null)
                    {
                        var resources = await resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
                        if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any()))
                        {
                            return new ConsentViewModel(model, returnUrl, request, client, resources);
                        }
                        else
                        {
                            logger.LogError("No scopes matching: {0}", request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
                        }
                    }
                    else
                    {
                        logger.LogError("Invalid client id: {0}", request.ClientId);
                    }
                }
                else
                {
                    logger.LogError("No consent request matching id: {0}", returnUrl);
                }
            }
            else
            {
                logger.LogError("No returnUrl passed");
            }

            return null;
        }
    }
}
