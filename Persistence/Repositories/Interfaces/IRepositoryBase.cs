using System.Linq.Expressions;

namespace Persistence.Repositories.Interfaces;

public interface IRepositoryBase<T>
{
    Task<T> CreateAsync(T entity);
    Task<IEnumerable<T>> CreateAsync(IEnumerable<T> entities);
    Task<T> FoundOrThrow(Expression<Func<T, bool>> predicate, Exception error);
    public Task<IEnumerable<T>> GetAsync(
       Expression<Func<T, bool>> filter = null,
       int first = 0, int offset = 0,
       params string[] navigationProperties);
    Task<List<T>> ToListAsync();
    Task<IList<T>> WhereAsync(Expression<Func<T, bool>> predicate, params string[] navigationProperties);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params string[] navigationProperties);
    Task<T> UpdateAsync(T updated);
    Task DeleteAsync(T entity);
    Task SoftDeleteAsync(T entity);

}
