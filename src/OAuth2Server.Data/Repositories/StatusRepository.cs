using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuth2Server.Data.Context;
using OAuth2Server.Data.Models.Entity;

namespace OAuth2Server.Data.Repositories
{
    public class StatusRepository : BaseRepository<ClientDbContext, Status>, IStatusRepository
    {
        public StatusRepository(ClientDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Status> GetSingle(Guid masterKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await base.FindBy(x => x.MasterKey == masterKey).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Status> GetSingle(int statusId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await base.FindBy(x => x.StatusId == statusId).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
