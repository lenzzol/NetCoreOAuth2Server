using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using OAuth2Server.Data.Models.Domain;
using OAuth2Server.Data.Models.Entity;

namespace OAuth2Server.Security.Managers
{
    public interface IAuthUserManager
    {
        Task<AuthUser> FindById(string id);

        Task CreateUser(UserDto user, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> UpdateUser(UserDto user, CancellationToken cancellationToken = default(CancellationToken), string passwordToken = null);

        Task<UserDto> GetAuthenticatedUser(HttpContext context);

        Task<UserDto> GetUser(Guid userId, CancellationToken cancellationToken = default(CancellationToken));

        Task<UserDto> GetUser(string email, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> ConfirmAccount(Guid userId, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> IsAccountConfirmed(Guid userId, CancellationToken cancellationToken = default(CancellationToken));

        Task<AuthenticationResult> Authenticate(string userName, string password, CancellationToken cancellationToken = default(CancellationToken));

        Task SendEmailInvite(Guid userId, CancellationToken cancellationToken = default(CancellationToken));

        Task SendEmailReset(Guid userId, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> EmailExists(string email, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> UserLogin(string userName, string password, bool rememberMe);
        Task UserLogout();
    }
}
