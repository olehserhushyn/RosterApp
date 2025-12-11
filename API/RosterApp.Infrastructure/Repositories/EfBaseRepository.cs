using Microsoft.EntityFrameworkCore;
using RosterApp.Domain.Entities;
using RosterApp.Infrastructure.Context;

namespace RosterApp.Infrastructure.Repositories
{
    public class EfBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<TEntity> _set;

        protected EfBaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _set = _dbContext.Set<TEntity>();
        }
    }
}
