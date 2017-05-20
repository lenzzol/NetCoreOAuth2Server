using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace OAuth2Server.Data.Repositories
{
    public abstract class BaseRepository<C,T> : IBaseRepository<T> where T : class where C : DbContext
    {
        protected BaseRepository(C dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<T>();
        }

        protected C DbContext { get; set; }
        protected DbSet<T> DbSet { get; set; }

        public virtual IQueryable<T> GetAll()
        {
            return this.DbSet;
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = this.DbSet.Where(predicate);
            return query;
        }

        public void Add(T entity)
        {
            EntityEntry<T> dbEntityEntry = this.DbContext.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                this.DbSet.Add(entity);
            }
        }

        public virtual void Update(T entity)
        {
            EntityEntry<T> dbEntityEntry = this.DbContext.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Detached)
            {
                this.DbSet.Attach(entity);
                this.DbSet.Update(entity);
            }

            dbEntityEntry.State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            EntityEntry<T> dbEntityEntry = this.DbContext.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                this.DbSet.Attach(entity);
                this.DbSet.Remove(entity);
            }
        }

        public virtual async Task SaveAsync(CancellationToken token = default(CancellationToken))
        {
            await this.DbContext.SaveChangesAsync(token);
        }
    }
}
