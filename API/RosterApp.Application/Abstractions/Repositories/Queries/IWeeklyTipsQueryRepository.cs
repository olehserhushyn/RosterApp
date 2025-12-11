using RosterApp.Domain.Entities;

namespace RosterApp.Application.Abstractions.Repositories.Queries
{
    public interface IWeeklyTipsQueryRepository
    {
        Task<WeeklyTips?> GetByIdAsync(int id);
        Task<WeeklyTips?> GetByWeekAsync(int weekNumber, int year);
        Task<IEnumerable<WeeklyTips>> GetAllAsync();
        Task<IEnumerable<WeeklyTips>> GetByYearAsync(int year);
    }
}
