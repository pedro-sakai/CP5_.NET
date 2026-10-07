using EletronicosStore.Domain.Entities;

namespace EletronicosStore.Application.Interfaces;

/// <summary>
/// Repositório específico de Produto, estendendo o repositório genérico com
/// consultas que exigem Include (ex.: trazer a Categoria junto).
/// </summary>
public interface IProdutoRepository : IRepository<Produto>
{
    /// <summary>Retorna todos os produtos já com a Categoria carregada.</summary>
    Task<IEnumerable<Produto>> GetAllComCategoriaAsync();

    /// <summary>Busca um produto pelo id já com a Categoria carregada.</summary>
    Task<Produto?> GetByIdComCategoriaAsync(int id);

    /// <summary>
    /// Retorna uma página de produtos (já com a Categoria carregada), ordenada por Id,
    /// cortada no banco (Skip/Take), junto com o total de produtos cadastrados.
    /// </summary>
    Task<(IEnumerable<Produto> Items, int TotalItems)> GetPagedComCategoriaAsync(int page, int pageSize);
}
