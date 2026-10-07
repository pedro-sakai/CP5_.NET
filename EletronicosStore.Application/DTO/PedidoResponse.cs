using EletronicosStore.Domain.Entities;

namespace EletronicosStore.Application.DTO;

/// <summary>DTO de saída para um item de pedido.</summary>
public record ItemPedidoResponse(int ProdutoId, string? ProdutoNome, int Quantidade, decimal PrecoUnitario, decimal Subtotal)
{
    public static ItemPedidoResponse FromDomain(ItemPedido item) => new(
        item.ProdutoId,
        item.Produto?.Nome,
        item.Quantidade,
        item.PrecoUnitario,
        item.Subtotal);
}

/// <summary>DTO de saída para pedido.</summary>
public record PedidoResponse(
    int Id,
    DateTime DataPedido,
    int ClienteId,
    string? ClienteNome,
    decimal ValorTotal,
    List<ItemPedidoResponse> Itens)
{
    public static PedidoResponse FromDomain(Pedido pedido) => new(
        pedido.Id,
        pedido.DataPedido,
        pedido.ClienteId,
        pedido.Cliente?.Nome,
        pedido.ValorTotal,
        pedido.Itens.Select(ItemPedidoResponse.FromDomain).ToList());
}
