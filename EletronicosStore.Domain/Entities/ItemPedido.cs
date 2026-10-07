using EletronicosStore.Domain.Common;

namespace EletronicosStore.Domain.Entities;

public class ItemPedido : BaseEntity
{
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }

    public int PedidoId { get; set; }
    public virtual Pedido Pedido { get; set; } = null!;

    public int ProdutoId { get; set; }
    public virtual Produto Produto { get; set; } = null!;

    /// <summary>Subtotal do item (quantidade x preço unitário no momento da compra).</summary>
    public decimal Subtotal => Quantidade * PrecoUnitario;
}
