using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System.Linq.Expressions;
using Transactions.Application.Repositories;
using Transactions.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Transactions.Persistence.Repositories;

public class Repository<T>(TransactionDbContext context) : IRepository<T> where T : BaseEntity<int>
{
    readonly DbSet<T> Table = context.Set<T>();
    public IQueryable<T> GetAll(bool tracking = true)
    {
        var query = Table.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return query;
    }

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true)
    {
        var query = Table.Where(method);

        if (!tracking)
            query = query.AsNoTracking();

        return query;
    }

    public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true)
    {
        var query = Table.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(method);
    }

    public async Task<T> GetByIdAsync(int id, bool tracking = true)
    {
        var query = Table.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(data => data.Id == id);
    }

    public async Task<bool> AddAsync(T model)
    {
        EntityEntry<T> entityEntry = await Table.AddAsync(model);
        return entityEntry.State == EntityState.Added;
    }

    public async Task<bool> AddRangeAsync(List<T> dataList)
    {
        await Table.AddRangeAsync(dataList);
        return true;
    }

    public bool Remove(T model)
    {
        EntityEntry<T> entityEntry = Table.Remove(model);
        return entityEntry.State == EntityState.Deleted;
    }

    public bool RemoveRange(List<T> dataList)
    {
        Table.RemoveRange(dataList);
        return true;
    }

    public async Task<bool> RemoveAsyncById(int id)
    {
        T model = await Table.FirstOrDefaultAsync(data => data.Id == id);
        return Remove(model);
    }

    public bool Update(T model)
    {
        EntityEntry<T> entityEntry = Table.Update(model);
        return entityEntry.State == EntityState.Modified;
    }

    public async Task<int> SaveAsync()
           => await context.SaveChangesAsync();

    public IDbContextTransaction BeginTransaction() => context.Database.BeginTransaction();

    public void CommitTransaction(IDbContextTransaction transaction) =>
        transaction.Commit();

    public void RollbackTransaction(IDbContextTransaction transaction) =>
        transaction.Rollback();
}