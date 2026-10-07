using EletronicosStore.Domain.Common;
using EletronicosStore.Domain.Exceptions;

namespace EletronicosStore.Domain.Entities;

public class Produto : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Estoque { get; set; }

    public int CategoriaId { get; set; }
    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();

    /// <summary>
    /// Baixa uma quantidade do estoque do produto.
    /// </summary>
    /// <exception cref="DomainException">
    /// Lançada quando a quantidade é inválida ou o estoque é insuficiente.
    /// </exception>
    public void BaixarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade a ser baixada do estoque deve ser maior que zero.");

        if (quantidade > Estoque)
            throw new DomainException($"Estoque insuficiente para o produto '{Nome}'. Disponível: {Estoque}, solicitado: {quantidade}.");

        Estoque -= quantidade;
    }
}
