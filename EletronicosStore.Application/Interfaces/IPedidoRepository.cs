using EletronicosStore.Domain.Entities;

namespace EletronicosStore.Application.Interfaces;

/// <summary>
/// Repositório específico de Pedido, estendendo o repositório genérico com
/// consultas que exigem Include (Cliente e Itens/Produto).
/// </summary>
public interface IPedidoRepository : IRepository<Pedido>
{
    Task<IEnumerable<Pedido>> GetAllComDetalhesAsync();
    Task<Pedido?> GetByIdComDetalhesAsync(int id);
}
