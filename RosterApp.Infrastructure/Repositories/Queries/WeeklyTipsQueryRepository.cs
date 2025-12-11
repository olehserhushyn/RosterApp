using Microsoft.EntityFrameworkCore;
using RosterApp.Application.Abstractions.Repositories.Queries;
using RosterApp.Domain.Entities;
using RosterApp.Infrastructure.Context;

namespace RosterApp.Infrastructure.Repositories.Queries
{

    public class WeeklyTipsQueryRepository : EfBaseRepository<WeeklyTips>, IWeeklyTipsQueryRepository
    {
        public WeeklyTipsQueryRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<WeeklyTips?> GetByIdAsync(int id)
        {
            return await _set
                .Include(w => w.Currency)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WeeklyTips?> GetByWeekAsync(int weekNumber, int year)
        {
            return await _set
                .Include(w => w.Currency)
                .FirstOrDefaultAsync(w => w.WeekNumber == weekNumber && w.Year == year);
        }

        public async Task<IEnumerable<WeeklyTips>> GetAllAsync()
        {
            return await _set
                .Include(w => w.Currency)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<WeeklyTips>> GetByYearAsync(int year)
        {
            return await _set
                .Include(w => w.Currency)
                .Where(w => w.Year == year)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
