using RosterApp.Domain.Entities;

namespace RosterApp.Application.Abstractions.Repositories.Queries
{
    public interface IEmployeeQueryRepository
    {
        Task<Employee?> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByEmailAsync(string email);
    }
}
