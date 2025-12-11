using RosterApp.Application.DTOs.Shifts.Responses;
using RosterApp.Domain.Entities;

namespace RosterApp.Application.Common.Mappers
{
    public static class ShiftMapper
    {
        public static ShiftDto MapToDto(Shift shift, string employeeFullName) =>
            new ShiftDto(
                shift.Id,
                shift.EmployeeId,
                employeeFullName,
                shift.Date,
                shift.StartTime,
                shift.EndTime,
                shift.HoursWorked,
                shift.Notes);
    }
}
