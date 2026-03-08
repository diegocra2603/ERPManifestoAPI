using System.Linq.Expressions;
using Domain.Contracts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class BaseRepository<T> : IAsyncRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public Task<IReadOnlyList<T>> GetAllAsync() =>
        _dbSet.AsNoTracking().ToListAsync().ContinueWith(t => (IReadOnlyList<T>)t.Result);

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.AsNoTracking().Where(predicate).ToListAsync();

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
        _dbSet.AnyAsync(predicate);

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
        _dbSet.FirstOrDefaultAsync(predicate);

    public Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate) =>
        _dbSet.FirstOrDefaultAsync(predicate);

    public async Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true)
    {
        IQueryable<T> query = _dbSet;
        if (disableTracking) query = query.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);
        if (predicate != null) query = query.Where(predicate);
        if (orderBy != null) query = orderBy(query);
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null,
        bool disableTracking = true)
    {
        IQueryable<T> query = _dbSet;
        if (disableTracking) query = query.AsNoTracking();
        if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
        if (predicate != null) query = query.Where(predicate);
        if (orderBy != null) query = orderBy(query);
        return await query.ToListAsync();
    }

    public void Add(T entity) => _dbSet.Add(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
}
