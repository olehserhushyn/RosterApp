namespace RosterApp.Application.DTOs.Shifts.Responses
{
    public record ShiftDto(
        int Id,
        int EmployeeId,
        string EmployeeFullName,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        double HoursWorked,
        string? Notes);
}
