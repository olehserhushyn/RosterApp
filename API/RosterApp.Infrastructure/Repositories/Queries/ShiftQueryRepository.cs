using Microsoft.EntityFrameworkCore;
using RosterApp.Application.Abstractions.Repositories.Queries;
using RosterApp.Domain.Entities;
using RosterApp.Infrastructure.Context;

namespace RosterApp.Infrastructure.Repositories.Queries
{
    public class ShiftQueryRepository : EfBaseRepository<Shift>, IShiftQueryRepository
    {
        public ShiftQueryRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Shift?> GetByIdAsync(int id)
        {
            return await _set
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Shift>> GetAllAsync()
        {
            return await _set
                .Include(s => s.Employee)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _set
                .Include(s => s.Employee)
                .Where(s => s.EmployeeId == employeeId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetByWeekAsync(int weekNumber, int year)
        {
            var weekStartDateTime = System.Globalization.ISOWeek.ToDateTime(year, weekNumber, DayOfWeek.Monday);
            var weekStart = DateOnly.FromDateTime(weekStartDateTime);
            var weekEnd = weekStart.AddDays(6);

            return await _set
                .Include(s => s.Employee)
                .Where(s => s.Date >= weekStart && s.Date <= weekEnd)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _set
                .Include(s => s.Employee)
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
