namespace RosterApp.Application.DTOs.WeeklyTips.Responses
{
    public record WeeklyRosterDto(
        int WeekNumber,
        int Year,
        DateOnly WeekStartDate,
        DateOnly WeekEndDate,
        List<EmployeeWeekSummaryDto> Employees);
}
