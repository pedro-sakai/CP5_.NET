using EletronicosStore.Domain.Entities;
using EletronicosStore.Domain.Exceptions;
using Xunit;

namespace EletronicosStore.Domain.Tests.Entities;

public class ProdutoTests
{
    [Fact]
    public void BaixarEstoque_ComQuantidadeDisponivel_ReduzEstoqueCorretamente()
    {
        // Arrange
        var produto = new Produto { Nome = "Mouse Gamer", Estoque = 10, Preco = 150m };

        // Act
        produto.BaixarEstoque(4);

        // Assert
        Assert.Equal(6, produto.Estoque);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void BaixarEstoque_ComQuantidadeMenorOuIgualAZero_LancaDomainException(int quantidadeInvalida)
    {
        // Arrange
        var produto = new Produto { Nome = "Teclado Mecânico", Estoque = 5, Preco = 300m };

        // Act & Assert
        Assert.Throws<DomainException>(() => produto.BaixarEstoque(quantidadeInvalida));
    }

    [Fact]
    public void BaixarEstoque_ComQuantidadeMaiorQueEstoqueDisponivel_LancaDomainException()
    {
        // Arrange
        var produto = new Produto { Nome = "Monitor 4K", Estoque = 2, Preco = 2000m };

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => produto.BaixarEstoque(5));
        Assert.Contains("Estoque insuficiente", exception.Message);
    }
}
