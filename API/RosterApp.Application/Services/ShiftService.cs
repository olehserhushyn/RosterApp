using RosterApp.Application.Abstractions;
using RosterApp.Application.Abstractions.Services;
using RosterApp.Application.Common.Mappers;
using RosterApp.Application.Common.Utils;
using RosterApp.Application.DTOs.Shifts.Requests;
using RosterApp.Application.DTOs.Shifts.Responses;
using RosterApp.Application.DTOs.WeeklyTips.Responses;
using RosterApp.Domain.Entities;

namespace RosterApp.Application.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShiftService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShiftDto> CreateShiftAsync(CreateShiftRequest request)
        {
            var employee = await _unitOfWork.EmployeeQueries.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {request.EmployeeId} not found");
            }

            var shift = new Shift(
                request.EmployeeId,
                request.Date,
                request.StartTime,
                request.EndTime,
                request.Notes);

            await _unitOfWork.ShiftCommands.AddAsync(shift);
            await _unitOfWork.SaveChangesAsync();

            return ShiftMapper.MapToDto(shift, employee.FirstName + " " + employee.LastName);
        }

        public async Task<ShiftDto> UpdateShiftAsync(int id, UpdateShiftRequest request)
        {
            var shift = await _unitOfWork.ShiftQueries.GetByIdAsync(id);
            if (shift == null)
            {
                throw new KeyNotFoundException($"Shift with ID {id} not found");
            }
                
            shift.Update(request.Date, request.StartTime, request.EndTime, request.Notes);

            await _unitOfWork.ShiftCommands.UpdateAsync(shift);
            await _unitOfWork.SaveChangesAsync();

            var employee = await _unitOfWork.EmployeeQueries.GetByIdAsync(shift.EmployeeId);

            return ShiftMapper.MapToDto(shift, employee?.FullName ?? "N/A");
        }

        public async Task DeleteShiftAsync(int id)
        {
            var shift = await _unitOfWork.ShiftQueries.GetByIdAsync(id);
            if (shift == null)
            {
                throw new KeyNotFoundException($"Shift with ID {id} not found");
            }

            shift.Delete();

            await _unitOfWork.ShiftCommands.UpdateAsync(shift);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ShiftDto?> GetShiftByIdAsync(int id)
        {
            var shift = await _unitOfWork.ShiftQueries.GetByIdAsync(id);
            if (shift == null)
            {
                return null;
            }

            var employee = await _unitOfWork.EmployeeQueries.GetByIdAsync(shift.EmployeeId);

            return ShiftMapper.MapToDto(shift, employee?.FullName ?? "N/A");
        }

        public async Task<IEnumerable<ShiftDto>> GetShiftsByWeekAsync(int weekNumber, int year)
        {
            var shifts = await _unitOfWork.ShiftQueries.GetByWeekAsync(weekNumber, year);
            var employees = await _unitOfWork.EmployeeQueries.GetAllAsync();
            var employeeDict = employees.ToDictionary(e => e.Id, e => e.FullName);

            return shifts.Select(s => ShiftMapper.MapToDto(s, employeeDict.GetValueOrDefault(s.EmployeeId, "N/A")));
        }

        public async Task<WeeklyRosterDto> GetWeeklyRosterAsync(int weekNumber, int year)
        {
            var shifts = await _unitOfWork.ShiftQueries.GetByWeekAsync(weekNumber, year);
            var employees = await _unitOfWork.EmployeeQueries.GetAllAsync();

            var weekStart = WeekUtils.GetWeekStartDate(weekNumber, year);
            var weekEnd = weekStart.AddDays(6);

            var employeeSummaries = employees.Select(emp =>
            {
                var empShifts = shifts.Where(s => s.EmployeeId == emp.Id).ToList();
                var totalHours = empShifts.Sum(s => s.HoursWorked);
                var shiftDtos = empShifts.Select(s => ShiftMapper.MapToDto(s, emp.FullName)).ToList();

                return new EmployeeWeekSummaryDto(emp.Id, emp.FullName, totalHours, shiftDtos);
            }).ToList();

            return new WeeklyRosterDto(weekNumber, year, weekStart, weekEnd, employeeSummaries);
        }
    }
}
