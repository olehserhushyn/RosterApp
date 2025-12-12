using RosterApp.Application.DTOs.Shifts.Responses;

namespace RosterApp.Application.DTOs.Employees.Responses
{
    public record EmployeeDetailsDto(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        DateTime CreatedAt,
        double TotalWeeklyHours,
        List<ShiftDto> Shifts);
}
