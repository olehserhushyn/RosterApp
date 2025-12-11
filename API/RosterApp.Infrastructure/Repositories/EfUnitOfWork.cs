using Microsoft.EntityFrameworkCore.Storage;
using RosterApp.Application.Abstractions;
using RosterApp.Application.Abstractions.Repositories.Commands;
using RosterApp.Application.Abstractions.Repositories.Queries;
using RosterApp.Infrastructure.Context;

namespace RosterApp.Infrastructure.Repositories
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;

        public IEmployeeCommandRepository EmployeeCommands { get; }
        public IShiftCommandRepository ShiftCommands { get; }
        public IWeeklyTipsCommandRepository WeeklyTipsCommands { get; }

        public IEmployeeQueryRepository EmployeeQueries { get; }
        public IShiftQueryRepository ShiftQueries { get; }
        public IWeeklyTipsQueryRepository WeeklyTipsQueries { get; }
        public ICurrencyQueryRepository CurrencyQueries { get; }

        public EfUnitOfWork(
            ApplicationDbContext dbContext,
            IEmployeeCommandRepository employeeCommands,
            IShiftCommandRepository shiftCommands,
            IWeeklyTipsCommandRepository weeklyTipsCommands,
            IEmployeeQueryRepository employeeQueries,
            IShiftQueryRepository shiftQueries,
            IWeeklyTipsQueryRepository weeklyTipsQueries,
            ICurrencyQueryRepository currencyQueries)
        {
            _dbContext = dbContext;

            EmployeeCommands = employeeCommands;
            ShiftCommands = shiftCommands;
            WeeklyTipsCommands = weeklyTipsCommands;

            EmployeeQueries = employeeQueries;
            ShiftQueries = shiftQueries;
            WeeklyTipsQueries = weeklyTipsQueries;
            CurrencyQueries = currencyQueries;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
            {
                return;
            }

            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
            {
                return;
            }

            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _dbContext.Dispose();
        }
    }
}
