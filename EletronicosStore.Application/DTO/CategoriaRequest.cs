using EletronicosStore.Domain.Entities;

namespace EletronicosStore.Application.DTO;

/// <summary>DTO de entrada para criação/atualização de uma categoria.</summary>
public class CategoriaRequest
{
    /// <summary>Nome da categoria.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Converte o DTO para a entidade de domínio <see cref="Categoria"/>.</summary>
    public Categoria ToDomain() => new() { Nome = Nome };
}
