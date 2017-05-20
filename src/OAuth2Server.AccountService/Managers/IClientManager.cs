using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OAuth2Server.Data.Models.Domain;

namespace OAuth2Server.AccountService.Managers
{
    public interface IClientManager
    {
        Task<IEnumerable<UserDto>> GetClientUsers(Guid clientAccountId, CancellationToken cancellationToken = default(CancellationToken));   
    }
}
