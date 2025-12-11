using RosterApp.Domain.Entities;

namespace RosterApp.Application.Abstractions.Repositories.Commands
{
    public interface IEmployeeCommandRepository
    {
        Task<Employee> AddAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
    }
}
