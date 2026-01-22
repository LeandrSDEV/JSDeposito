namespace JSDeposito.Core.Interfaces;

public interface IUnitOfWork
{
    Task BeginAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
