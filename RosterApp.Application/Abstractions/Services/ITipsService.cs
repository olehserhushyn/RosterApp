using RosterApp.Application.DTOs.WeeklyTips.Responses;

namespace RosterApp.Application.Abstractions.Services
{
    public interface ITipsService
    {
        Task<WeeklyTipsDto> GetWeeklyTips(int year, int weekNumber);
    }
}
