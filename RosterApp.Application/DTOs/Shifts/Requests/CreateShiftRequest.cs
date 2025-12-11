namespace RosterApp.Application.DTOs.Shifts.Requests
{
    public record CreateShiftRequest(
        int EmployeeId,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string? Notes);
}
