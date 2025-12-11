using RosterApp.Application.DTOs.WeeklyTips.Responses;

namespace RosterApp.Application.Abstractions.Services
{
    public interface ITipDistributionService
    {
        Task<TipDistributionDto> GetWeeklyDistributionAsync(int weekNumber, int year);
    }
}
