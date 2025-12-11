using RosterApp.Application.Abstractions.Repositories.Commands;
using RosterApp.Domain.Entities;
using RosterApp.Infrastructure.Context;

namespace RosterApp.Infrastructure.Repositories.Commands
{
    public class WeeklyTipsCommandRepository : EfBaseRepository<WeeklyTips>, IWeeklyTipsCommandRepository
    {
        public WeeklyTipsCommandRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<WeeklyTips> AddAsync(WeeklyTips weeklyTips)
        {
            if (weeklyTips == null)
            {
                throw new ArgumentNullException(nameof(weeklyTips));
            }

            await _set.AddAsync(weeklyTips);
            return weeklyTips;
        }

        public Task<WeeklyTips> UpdateAsync(WeeklyTips weeklyTips)
        {
            if (weeklyTips == null)
            {
                throw new ArgumentNullException(nameof(weeklyTips));
            }

            _set.Update(weeklyTips);
            return Task.FromResult(weeklyTips);
        }
    }
}
