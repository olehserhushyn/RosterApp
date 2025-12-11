namespace RosterApp.Application.DTOs.WeeklyTips.Responses
{
    public record EmployeeTipShareDto(
        int EmployeeId,
        string EmployeeName,
        double HoursWorked,
        decimal ShareAmount,
        double SharePercentage);
}
