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
            // If we already have employees, assume DB is seeded
            if (await _dbContext.Employees.AnyAsync(cancellationToken))
            {
                return;
            }

            // ========== CURRENCIES ==========
            var eur = new Currency("EUR", "€", "Euro");
            var usd = new Currency("USD", "$", "US Dollar");
            var gbp = new Currency("GBP", "£", "British Pound");

            _dbContext.Currencies.AddRange(eur, usd, gbp);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // ========== EMPLOYEES ==========
            var alice = new Employee("Alice", "Murphy", "alice@example.com");
            var bob = new Employee("Bob", "Ryan", "bob@example.com");
            var charlie = new Employee("Charlie", "Nolan", "charlie@example.com");
            var diana = new Employee("Diana", "Kelly", "diana@example.com");
            var ethan = new Employee("Ethan", "Walsh", "ethan@example.com");

            _dbContext.Employees.AddRange(alice, bob, charlie, diana, ethan);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // ========== WEEKLY TIPS + SHIFTS FOR MULTIPLE WEEKS ==========

            // Let's generate, say, last 8 weeks relative to "today"
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Find Monday of the current week
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var currentWeekStart = today.AddDays(-diff);

            var random = new Random();

            for (int i = 0; i < 8; i++)
            {
                var weekStart = currentWeekStart.AddDays(-7 * i);
                var weekDateTime = weekStart.ToDateTime(TimeOnly.MinValue);

                int year = weekDateTime.Year;
                int weekNumber = System.Globalization.ISOWeek.GetWeekOfYear(weekDateTime);

                // Total tips: between 200 and 600
                var totalTips = 200m + (decimal)random.Next(0, 400);

                var weeklyTips = new WeeklyTips(
                    weekNumber,
                    year,
                    weekStart,
                    eur.Id,          // use EUR for all demo tips
                    totalTips
                );

                _dbContext.WeeklyTips.Add(weeklyTips);

                // Some demo shifts for that week:
                // Alice: 2 shifts, Bob: 1 shift, Ethan: 1 shift, etc.

                var shifts = new List<Shift>
                {
                    new Shift(
                        alice.Id,
                        weekStart,                // Monday
                        new TimeOnly(9, 0),
                        new TimeOnly(17, 0),
                        "Alice full day"
                    ),
                    new Shift(
                        alice.Id,
                        weekStart.AddDays(1),     // Tuesday
                        new TimeOnly(9, 0),
                        new TimeOnly(13, 0),
                        "Alice half day"
                    ),
                    new Shift(
                        bob.Id,
                        weekStart.AddDays(2),     // Wednesday
                        new TimeOnly(12, 0),
                        new TimeOnly(20, 0),
                        "Bob evening shift"
                    ),
                    new Shift(
                        ethan.Id,
                        weekStart.AddDays(4),     // Friday
                        new TimeOnly(16, 0),
                        new TimeOnly(23, 0),
                        "Ethan late shift"
                    )
                };

                _dbContext.Shifts.AddRange(shifts);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}