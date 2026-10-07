using EletronicosStore.Domain.Common;

namespace EletronicosStore.Application.Interfaces;

/// <summary>
/// Contrato genérico de persistência, comum a todas as entidades do domínio
/// (qualquer classe que herde de <see cref="BaseEntity"/>).
/// </summary>
/// <typeparam name="T">Tipo da entidade, restrito a <see cref="BaseEntity"/>.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Retorna todos os registros (sem tracking).</summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>Busca um registro pelo id. Retorna null se não existir.</summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>Verifica se existe um registro com o id informado.</summary>
    Task<bool> ExistsByIdAsync(int id);

    /// <summary>Adiciona uma nova entidade (ainda não persiste — usar SaveChangesAsync).</summary>
    Task AddAsync(T entity);

    /// <summary>Marca uma entidade existente para atualização.</summary>
    void Update(T entity);

    /// <summary>Remove o registro pelo id. Retorna false se não existir.</summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Retorna uma página de registros (ordenados por Id, de forma estável) já cortada no banco
    /// (Skip/Take no IQueryable), junto com o total de registros existentes.
    /// </summary>
    /// <param name="page">Página solicitada (1-based).</param>
    /// <param name="pageSize">Tamanho da página.</param>
    Task<(IEnumerable<T> Items, int TotalItems)> GetPagedAsync(int page, int pageSize);

    /// <summary>Persiste as alterações pendentes no banco de dados.</summary>
    Task SaveChangesAsync();
}
