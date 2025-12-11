namespace RosterApp.Application.DTOs.Shifts.Requests
{
    public record UpdateShiftRequest(
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string? Notes);
}
