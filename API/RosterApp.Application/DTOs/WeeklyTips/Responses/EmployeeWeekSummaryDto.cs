using RosterApp.Application.DTOs.Shifts.Responses;

namespace RosterApp.Application.DTOs.WeeklyTips.Responses
{
    public record EmployeeWeekSummaryDto(
        int EmployeeId,
        string EmployeeFullName,
        double TotalHours,
        List<ShiftDto> Shifts);
}
