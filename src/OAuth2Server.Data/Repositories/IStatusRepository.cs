using System;
using System.Threading;
using System.Threading.Tasks;
using OAuth2Server.Data.Models.Entity;

namespace OAuth2Server.Data.Repositories
{
    public interface IStatusRepository : IBaseRepository<Status>
    {
        Task<Status> GetSingle(Guid masterKey, CancellationToken cancellationToken);

        Task<Status> GetSingle(int statusId, CancellationToken cancellationToken);
    }
}
