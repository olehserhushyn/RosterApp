using Moq;
using RosterApp.Application.Abstractions;
using RosterApp.Application.Abstractions.Repositories.Queries;
using RosterApp.Application.Services;
using RosterApp.Domain.Entities;

namespace RosterApp.Application.Test.TipDistributionServices.Tests
{
    public class TipDistributionServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IWeeklyTipsQueryRepository> _weeklyTipsQueryMock = new();
        private readonly Mock<ICurrencyQueryRepository> _currencyQueryMock = new();
        private readonly Mock<IShiftQueryRepository> _shiftQueryMock = new();
        private readonly Mock<IEmployeeQueryRepository> _employeeQueryMock = new();

        private TipDistributionService CreateService()
        {
            _uowMock.SetupGet(u => u.WeeklyTipsQueries).Returns(_weeklyTipsQueryMock.Object);
            _uowMock.SetupGet(u => u.CurrencyQueries).Returns(_currencyQueryMock.Object);
            _uowMock.SetupGet(u => u.ShiftQueries).Returns(_shiftQueryMock.Object);
            _uowMock.SetupGet(u => u.EmployeeQueries).Returns(_employeeQueryMock.Object);

            return new TipDistributionService(_uowMock.Object);
        }

        // ============= YOUR EXISTING TESTS (KEPT AS IS) =============

        [Fact]
        public async Task GetWeeklyDistributionAsync_SplitsTipsProportionally_ByHours()
        {
            // Arrange
            const int weekNumber = 1;
            const int year = 2025;
            const decimal totalTips = 300m;

            var weeklyTips = new WeeklyTips(
                weekNumber: weekNumber,
                year: year,
                weekStartDate: new DateOnly(2025, 1, 6),
                currencyId: 1,
                totalAmount: totalTips)
            { Id = 1 };

            var currency = new Currency("EUR", "€", "Euro");

            var alice = new Employee("Alice", "Murphy", "alice@example.com") { Id = 1 };
            var bob = new Employee("Bob", "Ryan", "bob@example.com") { Id = 2 };

            var shifts = new List<Shift>
            {
                // Alice: 20 hours (two 10h shifts)
                new Shift(alice.Id, new DateOnly(2025, 1, 6), new TimeOnly(9, 0), new TimeOnly(19, 0)) { Id = 1 },
                new Shift(alice.Id, new DateOnly(2025, 1, 7), new TimeOnly(9, 0), new TimeOnly(19, 0)) { Id = 2 },

                // Bob: 10 hours (one 10h shift)
                new Shift(bob.Id, new DateOnly(2025, 1, 8), new TimeOnly(10, 0), new TimeOnly(20, 0)) { Id = 3 }
            };

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(weeklyTips);

            _currencyQueryMock
                .Setup(r => r.GetByIdAsync(weeklyTips.CurrencyId))
                .ReturnsAsync(currency);

            _shiftQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(shifts);

            _employeeQueryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee> { alice, bob });

            var service = CreateService();

            // Act
            var result = await service.GetWeeklyDistributionAsync(weekNumber, year);

            // Assert
            Assert.Equal(totalTips, result.TotalTips);
            Assert.Equal("EUR", result.CurrencyCode);
            Assert.Equal("€", result.CurrencySymbol);

            // Total hours: 20 + 10 = 30
            Assert.Equal(30, result.TotalHours, precision: 10);

            var aliceShare = result.EmployeeShares.Single(e => e.EmployeeId == alice.Id);
            var bobShare = result.EmployeeShares.Single(e => e.EmployeeId == bob.Id);

            Assert.Equal(20, aliceShare.HoursWorked, 10);
            Assert.Equal(10, bobShare.HoursWorked, 10);

            Assert.Equal(200m, aliceShare.ShareAmount);
            Assert.Equal(100m, bobShare.ShareAmount);

            Assert.Equal(66.67, aliceShare.SharePercentage, 2);
            Assert.Equal(33.33, bobShare.SharePercentage, 2);
        }

        [Fact]
        public async Task GetWeeklyDistributionAsync_Throws_WhenWeeklyTipsNotFound()
        {
            // Arrange
            const int weekNumber = 2;
            const int year = 2025;

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync((WeeklyTips?)null);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.GetWeeklyDistributionAsync(weekNumber, year));
        }

        [Fact]
        public async Task GetWeeklyDistributionAsync_Throws_WhenCurrencyNotFound()
        {
            // Arrange
            const int weekNumber = 1;
            const int year = 2025;

            var weeklyTips = new WeeklyTips(
                weekNumber: weekNumber,
                year: year,
                weekStartDate: new DateOnly(2025, 1, 6),
                currencyId: 999, // bogus id
                totalAmount: 100m)
            { Id = 1 };

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(weeklyTips);

            _currencyQueryMock
                .Setup(r => r.GetByIdAsync(weeklyTips.CurrencyId))
                .ReturnsAsync((Currency?)null);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.GetWeeklyDistributionAsync(weekNumber, year));
        }

        [Fact]
        public async Task GetWeeklyDistributionAsync_ReturnsZeroShares_WhenNoShifts()
        {
            // Arrange
            const int weekNumber = 3;
            const int year = 2025;
            const decimal totalTips = 500m;

            var weeklyTips = new WeeklyTips(
                weekNumber: weekNumber,
                year: year,
                weekStartDate: new DateOnly(2025, 1, 20),
                currencyId: 1,
                totalAmount: totalTips)
            { Id = 1 };

            var currency = new Currency("EUR", "€", "Euro");

            var alice = new Employee("Alice", "Murphy", "alice@example.com") { Id = 1 };
            var bob = new Employee("Bob", "Ryan", "bob@example.com") { Id = 2 };

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(weeklyTips);

            _currencyQueryMock
                .Setup(r => r.GetByIdAsync(weeklyTips.CurrencyId))
                .ReturnsAsync(currency);

            _shiftQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(new List<Shift>());

            _employeeQueryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee> { alice, bob });

            var service = CreateService();

            // Act
            var result = await service.GetWeeklyDistributionAsync(weekNumber, year);

            // Assert
            Assert.Equal(0, result.TotalHours, 10);
            Assert.All(result.EmployeeShares, s =>
            {
                Assert.Equal(0, s.ShareAmount);
                Assert.Equal(0, s.SharePercentage, 10);
            });
        }

        // ============= NEW EDGE CASE TESTS =============

        [Fact]
        public async Task GetWeeklyDistributionAsync_HandlesEmployeeWithNoMatchingRecord_ReturnsNAsFullName()
        {
            // Arrange
            const int weekNumber = 1;
            const int year = 2025;
            const decimal totalTips = 1000m;

            var weeklyTips = new WeeklyTips(
                weekNumber: weekNumber,
                year: year,
                weekStartDate: new DateOnly(2025, 1, 6),
                currencyId: 1,
                totalAmount: totalTips)
            { Id = 1 };

            var currency = new Currency("USD", "$", "US Dollar");

            // Only Alice exists in employee repository
            var alice = new Employee("Alice", "Murphy", "alice@example.com") { Id = 1 };

            // But there's a shift for a non-existent employee (ID 999)
            var shifts = new List<Shift>
            {
                new Shift(alice.Id, new DateOnly(2025, 1, 6), new TimeOnly(9, 0), new TimeOnly(17, 0)) { Id = 1 },
                new Shift(999, new DateOnly(2025, 1, 7), new TimeOnly(9, 0), new TimeOnly(13, 0)) { Id = 2 } // Non-existent employee
            };

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(weeklyTips);

            _currencyQueryMock
                .Setup(r => r.GetByIdAsync(weeklyTips.CurrencyId))
                .ReturnsAsync(currency);

            _shiftQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(shifts);

            _employeeQueryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee> { alice }); // Only Alice exists

            var service = CreateService();

            // Act
            var result = await service.GetWeeklyDistributionAsync(weekNumber, year);

            // Assert
            Assert.Equal(2, result.EmployeeShares.Count);

            var aliceShare = result.EmployeeShares.Single(e => e.EmployeeId == alice.Id);
            var unknownShare = result.EmployeeShares.Single(e => e.EmployeeId == 999);

            Assert.Equal("Alice Murphy", aliceShare.EmployeeName);
            Assert.Equal("N/A", unknownShare.EmployeeName);
            Assert.Equal(8, aliceShare.HoursWorked); // 9-17 = 8 hours
            Assert.Equal(4, unknownShare.HoursWorked); // 9-13 = 4 hours
        }

        [Fact]
        public async Task GetWeeklyDistributionAsync_HandlesZeroTipsAmount_DistributesZeroToAll()
        {
            // Arrange
            const int weekNumber = 1;
            const int year = 2025;
            const decimal zeroTips = 0m;

            var weeklyTips = new WeeklyTips(
                weekNumber: weekNumber,
                year: year,
                weekStartDate: new DateOnly(2025, 1, 6),
                currencyId: 1,
                totalAmount: zeroTips)
            { Id = 1 };

            var currency = new Currency("EUR", "€", "Euro");

            var alice = new Employee("Alice", "Murphy", "alice@example.com") { Id = 1 };
            var bob = new Employee("Bob", "Ryan", "bob@example.com") { Id = 2 };

            var shifts = new List<Shift>
            {
                new Shift(alice.Id, new DateOnly(2025, 1, 6), new TimeOnly(9, 0), new TimeOnly(17, 0)) { Id = 1 },
                new Shift(bob.Id, new DateOnly(2025, 1, 7), new TimeOnly(10, 0), new TimeOnly(18, 0)) { Id = 2 }
            };

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(weeklyTips);

            _currencyQueryMock
                .Setup(r => r.GetByIdAsync(weeklyTips.CurrencyId))
                .ReturnsAsync(currency);

            _shiftQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(shifts);

            _employeeQueryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee> { alice, bob });

            var service = CreateService();

            // Act
            var result = await service.GetWeeklyDistributionAsync(weekNumber, year);

            // Assert
            Assert.Equal(0m, result.TotalTips);
            Assert.Equal(16, result.TotalHours); // 8 + 8 hours

            Assert.All(result.EmployeeShares, s =>
            {
                Assert.Equal(0m, s.ShareAmount);
                // But percentages should still be calculated based on hours
                Assert.True(s.SharePercentage > 0);
            });

            var aliceShare = result.EmployeeShares.Single(e => e.EmployeeId == alice.Id);
            var bobShare = result.EmployeeShares.Single(e => e.EmployeeId == bob.Id);

            // Both worked 8 hours, so 50% each
            Assert.Equal(50.00, aliceShare.SharePercentage, 2);
            Assert.Equal(50.00, bobShare.SharePercentage, 2);
        }

        [Fact]
        public async Task GetWeeklyDistributionAsync_HandlesLargeHourDifferences_CalculatesCorrectly()
        {
            // Arrange - Testing with one employee working very few hours
            const int weekNumber = 1;
            const int year = 2025;
            const decimal totalTips = 1000m;

            var weeklyTips = new WeeklyTips(
                weekNumber: weekNumber,
                year: year,
                weekStartDate: new DateOnly(2025, 1, 6),
                currencyId: 1,
                totalAmount: totalTips)
            { Id = 1 };

            var currency = new Currency("USD", "$", "US Dollar");

            var fullTimer = new Employee("John", "Doe", "john@example.com") { Id = 1 };
            var partTimer = new Employee("Jane", "Smith", "jane@example.com") { Id = 2 };

            var shifts = new List<Shift>
            {
                // Full-timer: 40 hours
                new Shift(fullTimer.Id, new DateOnly(2025, 1, 6), new TimeOnly(9, 0), new TimeOnly(17, 0)) { Id = 1 },
                new Shift(fullTimer.Id, new DateOnly(2025, 1, 7), new TimeOnly(9, 0), new TimeOnly(17, 0)) { Id = 2 },
                new Shift(fullTimer.Id, new DateOnly(2025, 1, 8), new TimeOnly(9, 0), new TimeOnly(17, 0)) { Id = 3 },
                new Shift(fullTimer.Id, new DateOnly(2025, 1, 9), new TimeOnly(9, 0), new TimeOnly(17, 0)) { Id = 4 },
                new Shift(fullTimer.Id, new DateOnly(2025, 1, 10), new TimeOnly(9, 0), new TimeOnly(17, 0)) { Id = 5 },
                
                // Part-timer: only 2 hours
                new Shift(partTimer.Id, new DateOnly(2025, 1, 6), new TimeOnly(17, 0), new TimeOnly(19, 0)) { Id = 6 }
            };

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(weeklyTips);

            _currencyQueryMock
                .Setup(r => r.GetByIdAsync(weeklyTips.CurrencyId))
                .ReturnsAsync(currency);

            _shiftQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(shifts);

            _employeeQueryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee> { fullTimer, partTimer });

            var service = CreateService();

            // Act
            var result = await service.GetWeeklyDistributionAsync(weekNumber, year);

            // Assert
            Assert.Equal(42, result.TotalHours); // 40 + 2

            var fullTimerShare = result.EmployeeShares.Single(e => e.EmployeeId == fullTimer.Id);
            var partTimerShare = result.EmployeeShares.Single(e => e.EmployeeId == partTimer.Id);

            // Full-timer: 40/42 = 95.238% of $1000 = $952.38
            Assert.Equal(40, fullTimerShare.HoursWorked);
            Assert.Equal(95.24, fullTimerShare.SharePercentage, 2); // Rounded
            Assert.Equal(952.38m, fullTimerShare.ShareAmount); // Rounded

            // Part-timer: 2/42 = 4.762% of $1000 = $47.62
            Assert.Equal(2, partTimerShare.HoursWorked);
            Assert.Equal(4.76, partTimerShare.SharePercentage, 2); // Rounded
            Assert.Equal(47.62m, partTimerShare.ShareAmount); // Rounded

            // Verify rounding doesn't lose money: 952.38 + 47.62 = 1000.00
            Assert.Equal(totalTips, fullTimerShare.ShareAmount + partTimerShare.ShareAmount);
        }

        [Fact]
        public async Task GetWeeklyDistributionAsync_HandlesDecimalHours_CalculatesCorrectly()
        {
            // Arrange - Testing with fractional hours (e.g., 7.5 hour shifts)
            const int weekNumber = 1;
            const int year = 2025;
            const decimal totalTips = 100m;

            var weeklyTips = new WeeklyTips(
                weekNumber: weekNumber,
                year: year,
                weekStartDate: new DateOnly(2025, 1, 6),
                currencyId: 1,
                totalAmount: totalTips)
            { Id = 1 };

            var currency = new Currency("EUR", "€", "Euro");

            var employee1 = new Employee("John", "Doe", "john@example.com") { Id = 1 };
            var employee2 = new Employee("Jane", "Smith", "jane@example.com") { Id = 2 };

            var shifts = new List<Shift>
            {
                // Employee 1: 7.5 hours (9:00-16:30)
                new Shift(employee1.Id, new DateOnly(2025, 1, 6), new TimeOnly(9, 0), new TimeOnly(16, 30)) { Id = 1 },
                
                // Employee 2: 6.25 hours (10:15-16:30)
                new Shift(employee2.Id, new DateOnly(2025, 1, 6), new TimeOnly(10, 15), new TimeOnly(16, 30)) { Id = 2 }
            };

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(weeklyTips);

            _currencyQueryMock
                .Setup(r => r.GetByIdAsync(weeklyTips.CurrencyId))
                .ReturnsAsync(currency);

            _shiftQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(shifts);

            _employeeQueryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee> { employee1, employee2 });

            var service = CreateService();

            // Act
            var result = await service.GetWeeklyDistributionAsync(weekNumber, year);

            // Assert
            Assert.Equal(13.75, result.TotalHours, 2); // 7.5 + 6.25

            var emp1Share = result.EmployeeShares.Single(e => e.EmployeeId == employee1.Id);
            var emp2Share = result.EmployeeShares.Single(e => e.EmployeeId == employee2.Id);

            // Employee 1: 7.5/13.75 = 54.545% of €100 = €54.55
            Assert.Equal(7.5, emp1Share.HoursWorked, 2);
            Assert.Equal(54.55, emp1Share.SharePercentage, 2);
            Assert.Equal(54.55m, emp1Share.ShareAmount);

            // Employee 2: 6.25/13.75 = 45.455% of €100 = €45.45
            Assert.Equal(6.25, emp2Share.HoursWorked, 2);
            Assert.Equal(45.45, emp2Share.SharePercentage, 2);
            Assert.Equal(45.45m, emp2Share.ShareAmount);

            // Check rounding: 54.55 + 45.45 = 100.00
            Assert.Equal(totalTips, emp1Share.ShareAmount + emp2Share.ShareAmount);
        }

        [Fact]
        public async Task GetWeeklyDistributionAsync_HandlesMultipleShiftsForSameEmployee_SumsHoursCorrectly()
        {
            // Arrange
            const int weekNumber = 1;
            const int year = 2025;
            const decimal totalTips = 500m;

            var weeklyTips = new WeeklyTips(
                weekNumber: weekNumber,
                year: year,
                weekStartDate: new DateOnly(2025, 1, 6),
                currencyId: 1,
                totalAmount: totalTips)
            { Id = 1 };

            var currency = new Currency("USD", "$", "US Dollar");

            var employee = new Employee("John", "Doe", "john@example.com") { Id = 1 };

            var shifts = new List<Shift>
            {
                // Same employee, multiple shifts in same week
                new Shift(employee.Id, new DateOnly(2025, 1, 6), new TimeOnly(9, 0), new TimeOnly(13, 0)) { Id = 1 }, // 4 hours
                new Shift(employee.Id, new DateOnly(2025, 1, 6), new TimeOnly(14, 0), new TimeOnly(18, 0)) { Id = 2 }, // 4 hours
                new Shift(employee.Id, new DateOnly(2025, 1, 7), new TimeOnly(10, 0), new TimeOnly(15, 0)) { Id = 3 }, // 5 hours
                new Shift(employee.Id, new DateOnly(2025, 1, 8), new TimeOnly(8, 0), new TimeOnly(16, 0)) { Id = 4 }  // 8 hours
            };

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(weeklyTips);

            _currencyQueryMock
                .Setup(r => r.GetByIdAsync(weeklyTips.CurrencyId))
                .ReturnsAsync(currency);

            _shiftQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(shifts);

            _employeeQueryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee> { employee });

            var service = CreateService();

            // Act
            var result = await service.GetWeeklyDistributionAsync(weekNumber, year);

            // Assert
            Assert.Equal(21, result.TotalHours); // 4+4+5+8 = 21
            Assert.Single(result.EmployeeShares);

            var share = result.EmployeeShares.First();
            Assert.Equal(21, share.HoursWorked);
            Assert.Equal(100.00, share.SharePercentage, 2);
            Assert.Equal(500m, share.ShareAmount);
        }

        [Fact]
        public async Task GetWeeklyDistributionAsync_HandlesShiftsForDeletedEmployees_IncludesThemInDistribution()
        {
            // Arrange
            const int weekNumber = 1;
            const int year = 2025;
            const decimal totalTips = 600m;

            var weeklyTips = new WeeklyTips(
                weekNumber: weekNumber,
                year: year,
                weekStartDate: new DateOnly(2025, 1, 6),
                currencyId: 1,
                totalAmount: totalTips)
            { Id = 1 };

            var currency = new Currency("EUR", "€", "Euro");

            // Employee repository returns only active employees
            var activeEmployee = new Employee("Active", "Employee", "active@example.com") { Id = 1 };

            // But there are shifts for a deleted employee (ID 2)
            var shifts = new List<Shift>
            {
                new Shift(1, new DateOnly(2025, 1, 6), new TimeOnly(9, 0), new TimeOnly(17, 0)) { Id = 1 },
                new Shift(2, new DateOnly(2025, 1, 7), new TimeOnly(10, 0), new TimeOnly(14, 0)) { Id = 2 } // Deleted employee
            };

            _weeklyTipsQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(weeklyTips);

            _currencyQueryMock
                .Setup(r => r.GetByIdAsync(weeklyTips.CurrencyId))
                .ReturnsAsync(currency);

            _shiftQueryMock
                .Setup(r => r.GetByWeekAsync(weekNumber, year))
                .ReturnsAsync(shifts);

            _employeeQueryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Employee> { activeEmployee }); // Only active employee

            var service = CreateService();

            // Act
            var result = await service.GetWeeklyDistributionAsync(weekNumber, year);

            // Assert
            Assert.Equal(2, result.EmployeeShares.Count);
            Assert.Equal(12, result.TotalHours); // 8 + 4

            var activeShare = result.EmployeeShares.Single(e => e.EmployeeId == 1);
            var deletedShare = result.EmployeeShares.Single(e => e.EmployeeId == 2);

            Assert.Equal("Active Employee", activeShare.EmployeeName);
            Assert.Equal("N/A", deletedShare.EmployeeName); // Shows "N/A" for deleted employee

            // Active: 8/12 = 66.67% of €600 = €400.00
            Assert.Equal(66.67, activeShare.SharePercentage, 2);
            Assert.Equal(400.00m, activeShare.ShareAmount);

            // Deleted: 4/12 = 33.33% of €600 = €200.00
            Assert.Equal(33.33, deletedShare.SharePercentage, 2);
            Assert.Equal(200.00m, deletedShare.ShareAmount);
        }
    }
}