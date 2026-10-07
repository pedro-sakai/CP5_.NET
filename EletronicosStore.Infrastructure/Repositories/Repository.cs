using EletronicosStore.Application.Interfaces;
using EletronicosStore.Domain.Common;
using EletronicosStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EletronicosStore.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica de <see cref="IRepository{T}"/> usando EF Core.
/// Serve qualquer entidade que herde de <see cref="BaseEntity"/>.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync() =>
        await DbSet.AsNoTracking().ToListAsync();

    public virtual async Task<T?> GetByIdAsync(int id) =>
        await DbSet.FindAsync(id);

    public virtual async Task<bool> ExistsByIdAsync(int id) =>
        await DbSet.AnyAsync(e => e.Id == id);

    public virtual async Task AddAsync(T entity) =>
        await DbSet.AddAsync(entity);

    public virtual void Update(T entity) =>
        DbSet.Update(entity);

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity is null)
            return false;

        DbSet.Remove(entity);
        return true;
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalItems)> GetPagedAsync(int page, int pageSize)
    {
        var query = DbSet.AsNoTracking().OrderBy(e => e.Id);

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public virtual async Task SaveChangesAsync() =>
        await Context.SaveChangesAsync();
}
