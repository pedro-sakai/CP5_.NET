using EletronicosStore.Application.Interfaces;
using EletronicosStore.Domain.Entities;
using EletronicosStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EletronicosStore.Infrastructure.Repositories;

/// <summary>
/// Repositório de Produto: herda o CRUD genérico de <see cref="Repository{T}"/>
/// e adiciona consultas específicas (com Include de Categoria).
/// </summary>
public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> GetAllComCategoriaAsync() =>
        await DbSet.Include(p => p.Categoria).AsNoTracking().ToListAsync();

    public async Task<Produto?> GetByIdComCategoriaAsync(int id) =>
        await DbSet.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<(IEnumerable<Produto> Items, int TotalItems)> GetPagedComCategoriaAsync(int page, int pageSize)
    {
        var query = DbSet.Include(p => p.Categoria).AsNoTracking().OrderBy(p => p.Id);

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }
}
