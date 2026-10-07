using EletronicosStore.Application.DTO;
using EletronicosStore.Application.Interfaces;
using EletronicosStore.Application.Services;
using EletronicosStore.Domain.Entities;
using EletronicosStore.Domain.Exceptions;
using Moq;
using Xunit;

namespace EletronicosStore.Application.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock = new();
    private readonly Mock<IRepository<Categoria>> _categoriaRepositoryMock = new();
    private readonly ProdutoService _sut; // System Under Test

    public ProdutoServiceTests()
    {
        _sut = new ProdutoService(_produtoRepositoryMock.Object, _categoriaRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ComCategoriaInexistente_LancaNotFoundExceptionENaoPersiste()
    {
        // Arrange
        var request = new ProdutoRequest
        {
            Nome = "Placa de Vídeo",
            Descricao = "RTX",
            Preco = 3500m,
            Estoque = 5,
            CategoriaId = 99
        };

        _categoriaRepositoryMock
            .Setup(r => r.ExistsByIdAsync(request.CategoriaId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateAsync(request));

        _produtoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Produto>()), Times.Never);
        _produtoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ComCategoriaExistente_PersisteProdutoUmaUnicaVez()
    {
        // Arrange
        var request = new ProdutoRequest
        {
            Nome = "SSD NVMe 1TB",
            Descricao = "Alta performance",
            Preco = 599.90m,
            Estoque = 20,
            CategoriaId = 1
        };

        _categoriaRepositoryMock
            .Setup(r => r.ExistsByIdAsync(request.CategoriaId))
            .ReturnsAsync(true);

        // Act
        var response = await _sut.CreateAsync(request);

        // Assert
        Assert.Equal(request.Nome, response.Nome);
        Assert.Equal(request.CategoriaId, response.CategoriaId);

        _produtoRepositoryMock.Verify(r => r.AddAsync(It.Is<Produto>(p => p.Nome == request.Nome)), Times.Once);
        _produtoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdInexistente_LancaNotFoundException()
    {
        // Arrange
        _produtoRepositoryMock
            .Setup(r => r.GetByIdComCategoriaAsync(It.IsAny<int>()))
            .ReturnsAsync((Produto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(id: 123));
    }

    [Theory]
    [InlineData(0, 20)]   // page abaixo do mínimo
    [InlineData(-1, 20)]  // page negativo
    [InlineData(1, 0)]    // pageSize abaixo do mínimo
    [InlineData(1, 101)]  // pageSize acima do teto
    [InlineData(1, 9999)] // pageSize muito acima do teto
    public async Task GetPagedAsync_ComPageOuPageSizeInvalido_LancaDomainException(int page, int pageSize)
    {
        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _sut.GetPagedAsync(page, pageSize));

        _produtoRepositoryMock.Verify(
            r => r.GetPagedComCategoriaAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_ComPageEPageSizeValidos_RetornaEnvelopeComTotaisCorretos()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            new() { Id = 3, Nome = "Monitor", Preco = 1200m, Estoque = 5, CategoriaId = 1 },
            new() { Id = 4, Nome = "Teclado", Preco = 250m, Estoque = 10, CategoriaId = 1 }
        };

        _produtoRepositoryMock
            .Setup(r => r.GetPagedComCategoriaAsync(2, 2))
            .ReturnsAsync((produtos, 5)); // 5 produtos no total, página 2 de 2 itens

        // Act
        var resultado = await _sut.GetPagedAsync(page: 2, pageSize: 2);

        // Assert
        Assert.Equal(2, resultado.Page);
        Assert.Equal(2, resultado.PageSize);
        Assert.Equal(5, resultado.TotalItems);
        Assert.Equal(3, resultado.TotalPages); // ceil(5/2) = 3
        Assert.True(resultado.HasPrevious);
        Assert.True(resultado.HasNext);
        Assert.Equal(2, resultado.Items.Count());
    }
}
