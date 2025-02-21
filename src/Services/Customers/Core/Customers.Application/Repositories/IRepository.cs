using Microsoft.EntityFrameworkCore.Storage;
using Shared.Models;
using System.Linq.Expressions;

namespace Customers.Application.Repositories;

public interface IRepository<T> where T : BaseEntity<int>
{
    IQueryable<T> GetAll(bool tracking = true);
    IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true);
    Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true);
    Task<T> GetByIdAsync(int id, bool tracking = true);
    Task<bool> AddAsync(T model);
    Task<bool> AddRangeAsync(List<T> dataList);
    bool Update(T model);
    bool Remove(T model);
    bool RemoveRange(List<T> dataList);
    Task<bool> RemoveAsyncById(int id);
    Task<int> SaveAsync();

    public IDbContextTransaction BeginTransaction();

    public void CommitTransaction(IDbContextTransaction transaction);

    public void RollbackTransaction(IDbContextTransaction transaction);
}