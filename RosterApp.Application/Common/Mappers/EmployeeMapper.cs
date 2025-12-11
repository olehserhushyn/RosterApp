using RosterApp.API.DTOs.Employees.Responses;
using RosterApp.Domain.Entities;

namespace RosterApp.Application.Common.Mappers
{
    public static class EmployeeMapper
    {
        public static EmployeeDto MapToDto(Employee employee) =>
           new EmployeeDto(
               employee.Id,
               employee.FirstName,
               employee.LastName,
               employee.Email,
               employee.CreatedAt);
    }
}
