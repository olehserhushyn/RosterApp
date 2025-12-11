using RosterApp.Domain.Entities;

namespace RosterApp.Application.Abstractions.Repositories.Commands
{
    public interface IWeeklyTipsCommandRepository
    {
        Task<WeeklyTips> AddAsync(WeeklyTips weeklyTips);
        Task<WeeklyTips> UpdateAsync(WeeklyTips weeklyTips);
    }
}
