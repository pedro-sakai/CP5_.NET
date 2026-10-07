using EletronicosStore.Domain.Common;
using EletronicosStore.Domain.Exceptions;

namespace EletronicosStore.Domain.Entities;

public class Pedido : BaseEntity
{
    public DateTime DataPedido { get; set; }

    public int ClienteId { get; set; }
    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();

    /// <summary>Valor total do pedido (soma dos subtotais dos itens).</summary>
    public decimal ValorTotal => Itens.Sum(i => i.Subtotal);

    /// <summary>
    /// Adiciona um item ao pedido, aplicando as regras de negócio do domínio.
    /// </summary>
    /// <param name="produtoId">Id do produto.</param>
    /// <param name="quantidade">Quantidade solicitada (deve ser maior que zero).</param>
    /// <param name="precoUnitario">Preço unitário vigente do produto (não pode ser negativo).</param>
    /// <exception cref="DomainException">
    /// Lançada quando a quantidade é menor ou igual a zero, ou o preço unitário é negativo.
    /// </exception>
    public void AdicionarItem(int produtoId, int quantidade, decimal precoUnitario)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade do item deve ser maior que zero.");

        if (precoUnitario < 0)
            throw new DomainException("O preço unitário não pode ser negativo.");

        Itens.Add(new ItemPedido
        {
            ProdutoId = produtoId,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario
        });
    }
}
