using RosterApp.Application.DTOs.Shifts.Requests;
using RosterApp.Application.DTOs.Shifts.Responses;
using RosterApp.Application.DTOs.WeeklyTips.Responses;

namespace RosterApp.Application.Abstractions.Services
{
    public interface IShiftService
    {
        Task<ShiftDto> CreateShiftAsync(CreateShiftRequest request);
        Task<ShiftDto> UpdateShiftAsync(int id, UpdateShiftRequest request);
        Task DeleteShiftAsync(int id);
        Task<ShiftDto?> GetShiftByIdAsync(int id);
        Task<IEnumerable<ShiftDto>> GetShiftsByWeekAsync(int weekNumber, int year);
        Task<WeeklyRosterDto> GetWeeklyRosterAsync(int weekNumber, int year);
    }
}
