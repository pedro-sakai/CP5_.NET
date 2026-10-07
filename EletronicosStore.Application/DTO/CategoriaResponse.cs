using EletronicosStore.Domain.Entities;

namespace EletronicosStore.Application.DTO;

/// <summary>DTO de saída para categoria.</summary>
public record CategoriaResponse(int Id, string Nome)
{
    public static CategoriaResponse FromDomain(Categoria categoria) =>
        new(categoria.Id, categoria.Nome);
}
