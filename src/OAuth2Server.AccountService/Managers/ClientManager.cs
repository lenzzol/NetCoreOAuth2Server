using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OAuth2Server.Data.Models.Domain;
using OAuth2Server.Data.Models.Entity;
using OAuth2Server.Data.Repositories;

namespace OAuth2Server.AccountService.Managers
{
    public class ClientManager : IClientManager
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger log;

        public ClientManager(IUserRepository userRepository, ILogger<ClientManager> logger)
        {
            this.userRepository = userRepository;
            this.log = logger;
        }

        public async Task<IEnumerable<UserDto>> GetClientUsers(Guid clientId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var clientUsers = new List<UserDto>();

            try
            {
                var users = await this.userRepository.GetUsersByClientAccount(clientId).OrderByDescending(x => x.CreatedDate).ToListAsync(cancellationToken);

                if (users != null)
                {
                    clientUsers.AddRange(users.Select(x => new UserDto
                    {
                        UserId = x.UserId,
                        UserName = x.UserName,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        ClientId = x.ClientId,
                        Status = x.Status.DisplayName,
                        FirstName = x.FirstName,
                        LastName = x.LastName
                    }));
                }

                return clientUsers;
            }
            catch (Exception ex)
            {
                this.log.LogError($"Error occurred getting client account users for client Id: {clientId}", ex);
                throw;
            }
        }
    }
}
