using RosterApp.API.DTOs.Employees.Requests;
using RosterApp.API.DTOs.Employees.Responses;
using RosterApp.Application.DTOs.Employees.Responses;

namespace RosterApp.Application.Abstractions.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request);
        Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request);
        Task DeleteEmployeeAsync(int id);
        Task<EmployeeDetailsDto?> GetEmployeeByIdAsync(int id, int weekNumber, int year);
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    }
}
