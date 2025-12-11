using RosterApp.Domain.Entities;
using RosterApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using RosterApp.Application.Abstractions.Repositories.Queries;

namespace RosterApp.Infrastructure.Repositories.Queries
{
    public class EmployeeQueryRepository : EfBaseRepository<Employee>, IEmployeeQueryRepository
    {
        public EmployeeQueryRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _set.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _set
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be empty.", nameof(email));
            }

            return await _set
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email == email);
        }
    }
}
