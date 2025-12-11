using RosterApp.Application.Abstractions.Repositories.Commands;
using RosterApp.Domain.Entities;
using RosterApp.Infrastructure.Context;

namespace RosterApp.Infrastructure.Repositories.Commands
{
    public class EmployeeCommandRepository : EfBaseRepository<Employee>, IEmployeeCommandRepository
    {
        public EmployeeCommandRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await _set.AddAsync(employee);
            return employee;
        }

        public Task<Employee> UpdateAsync(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            _set.Update(employee);
            return Task.FromResult(employee);
        }
    }
}
