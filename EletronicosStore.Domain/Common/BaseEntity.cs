namespace EletronicosStore.Domain.Common;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Centraliza o identificador para permitir o uso do repositório genérico (IRepository&lt;T&gt;).
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Identificador único da entidade.</summary>
    public int Id { get; set; }
}
