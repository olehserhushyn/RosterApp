using RosterApp.Domain.Entities;

namespace RosterApp.Application.Abstractions.Repositories.Commands
{
    public interface IShiftCommandRepository
    {
        Task<Shift> AddAsync(Shift shift);
        Task<Shift> UpdateAsync(Shift shift);
    }
}
