using RosterApp.Application.Abstractions;
using RosterApp.Application.Abstractions.Services;
using RosterApp.Application.DTOs.WeeklyTips.Responses;

namespace RosterApp.Application.Services
{
    public class TipDistributionService : ITipDistributionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TipDistributionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<TipDistributionDto> GetWeeklyDistributionAsync(int weekNumber, int year)
        {
            var weeklyTips = await _unitOfWork.WeeklyTipsQueries.GetByWeekAsync(weekNumber, year);
            if (weeklyTips == null)
            {
                throw new KeyNotFoundException($"No tips found for week {weekNumber} of {year}");
            }

            var currency = await _unitOfWork.CurrencyQueries.GetByIdAsync(weeklyTips.CurrencyId);
            if (currency == null)
            {
                throw new KeyNotFoundException($"Currency with ID {weeklyTips.CurrencyId} not found");
            }

            var shifts = await _unitOfWork.ShiftQueries.GetByWeekAsync(weekNumber, year);
            var employees = await _unitOfWork.EmployeeQueries.GetAllAsync();

            var employeeHours = shifts
                .GroupBy(s => s.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    TotalHours = g.Sum(s => s.HoursWorked)
                })
                .ToList();

            var totalHours = employeeHours.Sum(eh => eh.TotalHours);

            // Calculate tip distribution
            var employeeShares = employeeHours.Select(eh =>
            {
                var employee = employees.FirstOrDefault(e => e.Id == eh.EmployeeId);

                double shareFactor = totalHours > 0 ? eh.TotalHours / totalHours : 0.0;
                double sharePercentage = shareFactor * 100.0;
                decimal shareAmount = (decimal)shareFactor * weeklyTips.TotalAmount;

                return new EmployeeTipShareDto(
                    eh.EmployeeId,
                    employee?.FullName ?? "N/A",
                    eh.TotalHours,
                    Math.Round(shareAmount, 2),
                    Math.Round(sharePercentage, 2));
            }).ToList();

            return new TipDistributionDto(
                weekNumber,
                year,
                weeklyTips.WeekStartDate,
                weeklyTips.TotalAmount,
                currency.Code,
                currency.Symbol,
                totalHours,
                employeeShares);
        }
    }
}
