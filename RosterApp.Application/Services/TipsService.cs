using RosterApp.Application.Abstractions;
using RosterApp.Application.Abstractions.Services;
using RosterApp.Application.DTOs.WeeklyTips.Responses;

namespace RosterApp.Application.Services
{
    public class TipsService : ITipsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TipsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WeeklyTipsDto> GetWeeklyTips(int year, int weekNumber)
        {
            var weeklyTips = await _unitOfWork.WeeklyTipsQueries.GetByWeekAsync(weekNumber, year);

            if (weeklyTips == null)
            {
                throw new KeyNotFoundException($"No tips found for week {weekNumber} of {year}");
            }

            var currency = await _unitOfWork.CurrencyQueries.GetByIdAsync(
                weeklyTips.CurrencyId);

            return new WeeklyTipsDto(
                weeklyTips.Id,
                weeklyTips.WeekNumber,
                weeklyTips.Year,
                weeklyTips.WeekStartDate,
                weeklyTips.TotalAmount,
                currency?.Code ?? "EUR",
                currency?.Symbol ?? "€");
        }
    }
}
