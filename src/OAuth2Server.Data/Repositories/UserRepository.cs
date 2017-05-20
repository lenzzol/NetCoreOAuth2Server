using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using OAuth2Server.Data.Context;
using OAuth2Server.Data.Models.Entity;

namespace OAuth2Server.Data.Repositories
{
    public class UserRepository : BaseRepository<ClientDbContext, AuthUser>, IUserRepository
    {
        private readonly ILogger log;

        public UserRepository(ClientDbContext dbContext, IStatusRepository statusRepository, ILogger<UserRepository> logger) : base(dbContext)
        {
            this.StatusRepository = statusRepository;
            this.log = logger;
        }

        private IStatusRepository StatusRepository { get; set; }

        public async Task<AuthUser> GetSingle(Guid userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await base.FindBy(x => x.UserId == userId).FirstAsync(cancellationToken);
        }

        public async Task<AuthUser> GetSingle(string username,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await base.FindBy(x => x.UserName == username).FirstOrDefaultAsync(cancellationToken);
        }

        public IQueryable<AuthUser> GetUsersByClientAccount(Guid clientId)
        {
            try
            {
                var users = base.FindBy(x => x.ClientId == clientId);
                var statuses = this.StatusRepository.GetAll();

                var usersFull = from u in users
                                join s in statuses on u.StatusId equals s.StatusId
                                select new AuthUser
                                {
                                    UserId = u.UserId,
                                    UserName = u.UserName,
                                    Email = u.Email,
                                    EmailConfirmed = u.EmailConfirmed,
                                    PasswordHash = u.PasswordHash,
                                    ConcurrencyStamp = u.ConcurrencyStamp,
                                    PhoneNumber = u.PhoneNumber,
                                    PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                                    TwoFactorEnabled = u.TwoFactorEnabled,
                                    LockoutEnd = u.LockoutEnd,
                                    LockoutEnabled = u.LockoutEnabled,
                                    AccessFailedCount = u.AccessFailedCount,
                                    ClientId = u.ClientId,
                                    StatusId = u.StatusId,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    RecordVersion = u.RecordVersion,
                                    CreatedDate = u.CreatedDate,
                                    CreatedByUserId = u.CreatedByUserId,
                                    UpdatedDate = u.UpdatedDate,
                                    UpdatedByUserId = u.UpdatedByUserId,
                                    Status = s
                                };

                return usersFull;
            }
            catch (Exception ex)
            {
                this.log.LogError($"Error occurred querying users for client id: {clientId}", ex);
                throw;
            }
        }

        public override void Update(AuthUser entity)
        {
            EntityEntry<AuthUser> dbEntityEntry = base.DbContext.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Detached)
            {
                base.DbSet.Attach(entity);
                entity.ConcurrencyStamp = Guid.NewGuid().ToString();
                base.DbSet.Update(entity);
            }

            dbEntityEntry.State = EntityState.Modified;
        }
    }
}
