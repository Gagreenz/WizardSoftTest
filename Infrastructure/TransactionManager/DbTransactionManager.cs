using Microsoft.EntityFrameworkCore;
using WizardSoftTestService.Db;

namespace WizardSoftTestService.Infrastructure.TransactionManager;

public class DbTransactionManager : IDbTransactionManager
{
    private readonly DbContext _db;

    public DbTransactionManager(AppDbContext db)
    {
        _db = db;
    }

    public async Task<T> RunAsync<T>(Func<Task<T>> action)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var result = await action();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task RunAsync(Func<Task> action)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
