using RosterApp.Application.Abstractions.Repositories.Commands;
using RosterApp.Application.Abstractions.Repositories.Queries;

namespace RosterApp.Application.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeCommandRepository EmployeeCommands { get; }
        IShiftCommandRepository ShiftCommands { get; }
        IWeeklyTipsCommandRepository WeeklyTipsCommands { get; }

        IEmployeeQueryRepository EmployeeQueries { get; }
        IShiftQueryRepository ShiftQueries { get; }
        IWeeklyTipsQueryRepository WeeklyTipsQueries { get; }
        ICurrencyQueryRepository CurrencyQueries { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
