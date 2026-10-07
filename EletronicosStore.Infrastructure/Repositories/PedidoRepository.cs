using EletronicosStore.Application.Interfaces;
using EletronicosStore.Domain.Entities;
using EletronicosStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EletronicosStore.Infrastructure.Repositories;

/// <summary>
/// Repositório de Pedido: herda o CRUD genérico de <see cref="Repository{T}"/>
/// e adiciona consultas específicas (com Include de Cliente e Itens/Produto).
/// </summary>
public class PedidoRepository : Repository<Pedido>, IPedidoRepository
{
    public PedidoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Pedido>> GetAllComDetalhesAsync() =>
        await DbSet
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Pedido?> GetByIdComDetalhesAsync(int id) =>
        await DbSet
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);
}
