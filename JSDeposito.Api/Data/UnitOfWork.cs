using JSDeposito.Api.Data;
using JSDeposito.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;


public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task BeginAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
        await _transaction!.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction!.RollbackAsync();
    }
}

