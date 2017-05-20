using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OAuth2Server.AccountService.Managers;
using OAuth2Server.Security.Managers;

namespace OAuth2Server.Api.Apps.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly IClientManager clientManager;
        private readonly IAuthUserManager userManager;
        private readonly ILogger log;

        public ClientController(IClientManager clientManager, IAuthUserManager userManager, ILogger<ClientController> logger)
        {
            this.clientManager = clientManager;
            this.userManager = userManager;
            this.log = logger;
        }

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetClientUsers(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var authUser = await this.userManager.GetAuthenticatedUser(this.HttpContext);

                if (authUser != null)
                {
                    var clientId = Guid.Parse(id);
                    return Ok(await this.clientManager.GetClientUsers(clientId, cancellationToken));
                }
                else
                {
                    return base.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                this.log.LogError($"Error occurred retreiving client users for client Id: {id}", ex);
                return base.BadRequest();
            }
        }
    }
}
