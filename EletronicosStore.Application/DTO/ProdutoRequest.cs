namespace EletronicosStore.Application.DTO;

/// <summary>DTO de entrada para criação/atualização de um produto.</summary>
public class ProdutoRequest
{
    /// <summary>Nome do produto.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Descrição do produto.</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Preço unitário de venda.</summary>
    public decimal Preco { get; set; }

    /// <summary>Quantidade disponível em estoque.</summary>
    public int Estoque { get; set; }

    /// <summary>Id da categoria à qual o produto pertence.</summary>
    public int CategoriaId { get; set; }
}
