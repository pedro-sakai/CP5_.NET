using EletronicosStore.Domain.Entities;

namespace EletronicosStore.Application.DTO;

/// <summary>DTO de saída para produto.</summary>
public record ProdutoResponse(
    int Id,
    string Nome,
    string Descricao,
    decimal Preco,
    int Estoque,
    int CategoriaId,
    string? CategoriaNome)
{
    public static ProdutoResponse FromDomain(Produto produto) => new(
        produto.Id,
        produto.Nome,
        produto.Descricao,
        produto.Preco,
        produto.Estoque,
        produto.CategoriaId,
        produto.Categoria?.Nome);
}
