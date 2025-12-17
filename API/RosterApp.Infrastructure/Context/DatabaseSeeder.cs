using Microsoft.EntityFrameworkCore;
using RosterApp.Domain.Entities;

namespace RosterApp.Infrastructure.Context
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken = default);
    }

    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ApplicationDbContext _dbContext;

        public DatabaseSeeder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (!await _dbContext.Currencies.AnyAsync(cancellationToken))
            {
                var eur = new Currency("EUR", "€", "Euro");
                var usd = new Currency("USD", "$", "US Dollar");
                var gbp = new Currency("GBP", "£", "British Pound");

                _dbContext.Currencies.AddRange(eur, usd, gbp);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            if (!await _dbContext.Employees.AnyAsync(cancellationToken))
            {
                var alice = new Employee("Alice", "Murphy", "alice@example.com");
                var bob = new Employee("Bob", "Ryan", "bob@example.com");
                var charlie = new Employee("Charlie", "Nolan", "charlie@example.com");
                var diana = new Employee("Diana", "Kelly", "diana@example.com");
                var ethan = new Employee("Ethan", "Walsh", "ethan@example.com");

                _dbContext.Employees.AddRange(alice, bob, charlie, diana, ethan);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
 
            var employees = await _dbContext.Employees.AsNoTracking().ToListAsync(cancellationToken);
            var aliceEmp = employees.First(e => e.Email == "alice@example.com");
            var bobEmp = employees.First(e => e.Email == "bob@example.com");
            var ethanEmp = employees.First(e => e.Email == "ethan@example.com");

            var eurId = await _dbContext.Currencies
                .AsNoTracking()
                .Where(c => c.Code == "EUR")
                .Select(c => c.Id)
                .SingleAsync(cancellationToken);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var currentWeekStart = today.AddDays(-diff);

            var random = new Random();

            for (int i = 0; i < 8; i++)
            {
                var weekStart = currentWeekStart.AddDays(-7 * i);
                var weekStartDateTime = weekStart.ToDateTime(TimeOnly.MinValue);

                int isoYear = System.Globalization.ISOWeek.GetYear(weekStartDateTime);
                int weekNumber = System.Globalization.ISOWeek.GetWeekOfYear(weekStartDateTime);

                bool weeklyTipsExists = await _dbContext.WeeklyTips.AnyAsync(w =>
                    w.WeekNumber == weekNumber &&
                    w.Year == isoYear &&
                    w.WeekStartDate == weekStart, cancellationToken);

                if (!weeklyTipsExists)
                {
                    var totalTips = 200m + (decimal)random.Next(0, 400);

                    var weeklyTips = new WeeklyTips(
                        weekNumber,
                        isoYear,
                        weekStart,
                        eurId,
                        totalTips
                    );

                    _dbContext.WeeklyTips.Add(weeklyTips);
                }

                bool shiftsExistForWeek = await _dbContext.Shifts.AnyAsync(s =>
                    s.Date >= weekStart &&
                    s.Date <= weekStart.AddDays(6), cancellationToken);

                if (!shiftsExistForWeek)
                {
                    var shifts = new List<Shift>
            {
                new Shift(
                    aliceEmp.Id,
                    weekStart,                // Monday
                    new TimeOnly(9, 0),
                    new TimeOnly(17, 0),
                    "Alice full day"
                ),
                new Shift(
                    aliceEmp.Id,
                    weekStart.AddDays(1),     // Tuesday
                    new TimeOnly(9, 0),
                    new TimeOnly(13, 0),
                    "Alice half day"
                ),
                new Shift(
                    bobEmp.Id,
                    weekStart.AddDays(2),     // Wednesday
                    new TimeOnly(12, 0),
                    new TimeOnly(20, 0),
                    "Bob evening shift"
                ),
                new Shift(
                    ethanEmp.Id,
                    weekStart.AddDays(4),     // Friday
                    new TimeOnly(16, 0),
                    new TimeOnly(23, 0),
                    "Ethan late shift"
                )
            };

                    _dbContext.Shifts.AddRange(shifts);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}