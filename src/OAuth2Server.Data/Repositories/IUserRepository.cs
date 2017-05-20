using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OAuth2Server.Data.Models.Entity;

namespace OAuth2Server.Data.Repositories
{
    public interface IUserRepository : IBaseRepository<AuthUser>
    {
        Task<AuthUser> GetSingle(Guid userId, CancellationToken cancellationToken);
        Task<AuthUser> GetSingle(string username, CancellationToken cancellationToken = default(CancellationToken));
        IQueryable<AuthUser> GetUsersByClientAccount(Guid clientAccountId);
    }
}
