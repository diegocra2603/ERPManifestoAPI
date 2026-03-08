using System.Linq.Expressions;

namespace Domain.Contracts.Infrastructure.Persistence;

public interface IAsyncRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate);

    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null,
                                   Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                   string? includeString = null,
                                   bool disableTracking = true);
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null,
                                   Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                   List<Expression<Func<T, object>>>? includes = null,
                                   bool disableTracking = true);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}
