using OAuth2Server.Data.Models.Entity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth2Server.Data.Repositories
{
    public interface IClientRepository : IBaseRepository<Client>
    {
        Task<Client> GetSingle(Guid clientId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
