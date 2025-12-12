using RosterApp.Domain.Entities;

namespace RosterApp.Application.Abstractions.Repositories.Queries
{
    public interface IShiftQueryRepository
    {
        Task<Shift?> GetByIdAsync(int id);
        Task<IEnumerable<Shift>> GetAllAsync();
        Task<IEnumerable<Shift>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<Shift>> GetByWeekAsync(int weekNumber, int year);
        Task<IEnumerable<Shift>> GetByWeekAndEmployeeAsync(int employeeId, int weekNumber, int year);
        Task<IEnumerable<Shift>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    }
}
