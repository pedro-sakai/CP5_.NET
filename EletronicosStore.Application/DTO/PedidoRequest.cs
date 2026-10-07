namespace EletronicosStore.Application.DTO;

/// <summary>DTO de entrada para um item do pedido.</summary>
public class ItemPedidoRequest
{
    /// <summary>Id do produto solicitado.</summary>
    public int ProdutoId { get; set; }

    /// <summary>Quantidade solicitada (deve ser maior que zero).</summary>
    public int Quantidade { get; set; }
}

/// <summary>DTO de entrada para criação de um pedido.</summary>
public class PedidoRequest
{
    /// <summary>Id do cliente que está realizando o pedido.</summary>
    public int ClienteId { get; set; }

    /// <summary>Itens do pedido.</summary>
    public List<ItemPedidoRequest> Itens { get; set; } = new();
}
