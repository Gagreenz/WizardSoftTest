namespace WizardSoftTestService.Infrastructure.TransactionManager;

public interface IDbTransactionManager
{
    Task<T> RunAsync<T>(Func<Task<T>> action);
    Task RunAsync(Func<Task> action);
}
