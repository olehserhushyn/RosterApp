using RosterApp.Domain.Entities;

namespace RosterApp.Application.Abstractions.Repositories.Queries
{
    public interface ICurrencyQueryRepository
    {
        Task<Currency?> GetByIdAsync(int id);
        Task<Currency?> GetByCodeAsync(string code);
        Task<IEnumerable<Currency>> GetAllAsync();
    }
}
