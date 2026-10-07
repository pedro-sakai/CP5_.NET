using EletronicosStore.Domain.Entities;
using EletronicosStore.Domain.Exceptions;
using Xunit;

namespace EletronicosStore.Domain.Tests.Entities;

public class PedidoTests
{
    [Fact]
    public void AdicionarItem_ComQuantidadeEPrecoValidos_AdicionaItemEAtualizaValorTotal()
    {
        // Arrange
        var pedido = new Pedido { ClienteId = 1, DataPedido = DateTime.Now };

        // Act
        pedido.AdicionarItem(produtoId: 10, quantidade: 3, precoUnitario: 25.50m);

        // Assert
        Assert.Single(pedido.Itens);
        Assert.Equal(76.50m, pedido.ValorTotal);
        Assert.Equal(10, pedido.Itens.First().ProdutoId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void AdicionarItem_ComQuantidadeMenorOuIgualAZero_LancaDomainException(int quantidadeInvalida)
    {
        // Arrange
        var pedido = new Pedido { ClienteId = 1, DataPedido = DateTime.Now };

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            pedido.AdicionarItem(produtoId: 1, quantidade: quantidadeInvalida, precoUnitario: 10m));

        Assert.Contains("quantidade", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(pedido.Itens);
    }

    [Fact]
    public void AdicionarItem_ComPrecoUnitarioNegativo_LancaDomainException()
    {
        // Arrange
        var pedido = new Pedido { ClienteId = 1, DataPedido = DateTime.Now };

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            pedido.AdicionarItem(produtoId: 1, quantidade: 1, precoUnitario: -5m));
    }
}
