using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OAuth2Server.Data.Models.Domain;
using OAuth2Server.Security.Managers;

namespace OAuth2Server.Api.Apps.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IAuthUserManager authUserManager;
        private readonly ILogger log;

        public UserController(IAuthUserManager authUserManager, ILogger<UserController> logger)
        {
            this.authUserManager = authUserManager;
            this.log = logger;
        }

        // POST api/user
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto user, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await this.authUserManager.CreateUser(user, cancellationToken);
                return base.Ok();
            }
            catch (Exception ex)
            {
                this.log.LogError("Error occurred during Create User web request: ", ex);
                return base.BadRequest();
            }
        }

        // PUT api/user/{userId}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UserDto user, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await this.authUserManager.UpdateUser(user, cancellationToken);
                return base.Ok();
            }
            catch (Exception ex)
            {
                this.log.LogError("Error occurred during Update User web request: ", ex);
                return base.BadRequest();
            }
        }

        // POST api/user/{userId}/invite
        [HttpPost("{id}/invite")]
        public async Task<IActionResult> ResendInvite(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException(nameof(id));
                }

                Guid userId;
                var result = Guid.TryParse(id, out userId);
                
                if (!result)
                {
                    throw new FormatException($"Could not parse id: {id}");
                }

                await this.authUserManager.SendEmailInvite(userId, cancellationToken);
                return base.Ok();
            }
            catch (Exception ex)
            {
                this.log.LogError($"Error occurred during Resend Invite web request for userid: {id}", ex);
                return base.BadRequest();
            }
        }

        // POST api/user/{userId}/reset
        [HttpPost("{id}/reset")]
        public async Task<IActionResult> ResetPassword(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException(nameof(id));
                }

                Guid userId;
                var result = Guid.TryParse(id, out userId);

                if (!result)
                {
                    throw new FormatException($"Could not parse id: {id}");
                }

                await this.authUserManager.SendEmailReset(userId, cancellationToken);
                return base.Ok();
            }
            catch (Exception ex)
            {
                this.log.LogError($"Error occurred during Resend Invite web request for userid: {id}", ex);
                return base.BadRequest();
            }
        }
    }
}
