using Microsoft.EntityFrameworkCore;
using RosterApp.Application.Abstractions.Repositories.Queries;
using RosterApp.Domain.Entities;
using RosterApp.Infrastructure.Context;

namespace RosterApp.Infrastructure.Repositories.Queries
{
    public class CurrencyQueryRepository : EfBaseRepository<Currency>, ICurrencyQueryRepository
    {
        public CurrencyQueryRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Currency?> GetByIdAsync(int id)
        {
            return await _set.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Currency?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Currency code cannot be empty.", nameof(code));
            }

            code = code.ToUpperInvariant();

            return await _set
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            return await _set
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
