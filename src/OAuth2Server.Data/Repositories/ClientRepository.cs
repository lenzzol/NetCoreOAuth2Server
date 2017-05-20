using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAuth2Server.Data.Context;
using OAuth2Server.Data.Models.Entity;

namespace OAuth2Server.Data.Repositories
{
    public class ClientRepository : BaseRepository<ClientDbContext, Client>, IClientRepository
    {
        private readonly IStatusRepository statusRepository;

        public ClientRepository(ClientDbContext dbContext, IStatusRepository statusRepository) : base(dbContext)
        {
            this.statusRepository = statusRepository;
        }

        public Task<Client> GetSingle(Guid clientId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.FindBy(x => x.ClientId == clientId).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
