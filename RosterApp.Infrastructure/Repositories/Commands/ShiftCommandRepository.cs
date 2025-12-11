using RosterApp.Application.Abstractions.Repositories.Commands;
using RosterApp.Domain.Entities;
using RosterApp.Infrastructure.Context;

namespace RosterApp.Infrastructure.Repositories.Commands
{
    public class ShiftCommandRepository : EfBaseRepository<Shift>, IShiftCommandRepository
    {
        public ShiftCommandRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Shift> AddAsync(Shift shift)
        {
            if (shift == null)
            {
                throw new ArgumentNullException(nameof(shift));
            }

            await _set.AddAsync(shift);
            return shift;
        }

        public Task<Shift> UpdateAsync(Shift shift)
        {
            if (shift == null)
            {
                throw new ArgumentNullException(nameof(shift));
            }

            _set.Update(shift);
            return Task.FromResult(shift);
        }
    }
}
