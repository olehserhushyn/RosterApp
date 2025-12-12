using RosterApp.API.DTOs.Employees.Requests;
using RosterApp.API.DTOs.Employees.Responses;
using RosterApp.Application.Abstractions;
using RosterApp.Application.Abstractions.Services;
using RosterApp.Application.Common.Mappers;
using RosterApp.Application.DTOs.Employees.Responses;
using RosterApp.Domain.Entities;

namespace RosterApp.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            var employee = new Employee(request.FirstName, request.LastName, request.Email);
            await _unitOfWork.EmployeeCommands.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return EmployeeMapper.MapToDto(employee);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request)
        {
            var employee = await _unitOfWork.EmployeeQueries.GetByIdAsync(id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {id} not found");
            }
                

            employee.UpdateDetails(request.FirstName, request.LastName, request.Email);
            await _unitOfWork.EmployeeCommands.UpdateAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return EmployeeMapper.MapToDto(employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.EmployeeQueries.GetByIdAsync(id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {id} not found");
            }

            employee.Delete();

            await _unitOfWork.EmployeeCommands.UpdateAsync(employee);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<EmployeeDetailsDto?> GetEmployeeByIdAsync(int id, int weekNumber, int year)
        {
            var employee = await _unitOfWork.EmployeeQueries.GetByIdAsync(id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {id} not found");
            }

            var shifts = await _unitOfWork.ShiftQueries.GetByWeekAndEmployeeAsync(id, weekNumber, year);

            double totalHours = shifts.Sum(shift => (shift.EndTime - shift.StartTime).TotalHours);
            var shiftDtos = shifts.Select(s => ShiftMapper.MapToDto(s, employee.FullName));

            return employee != null ? EmployeeMapper.MapToDetailsDto(employee, totalHours, shiftDtos) : null;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _unitOfWork.EmployeeQueries.GetAllAsync();
            return employees.Select(EmployeeMapper.MapToDto);
        }  
    }
}
