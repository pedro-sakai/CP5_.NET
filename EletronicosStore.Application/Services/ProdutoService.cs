using EletronicosStore.Application.DTO;
using EletronicosStore.Application.Interfaces;
using EletronicosStore.Application.Interfaces.Services;
using EletronicosStore.Domain.Entities;
using EletronicosStore.Domain.Exceptions;

namespace EletronicosStore.Application.Services;

/// <summary>
/// Implementação da camada de serviço de Produto.
/// Usa o repositório específico de Produto (para consultas com Include) e o
/// repositório genérico de Categoria (para validar a existência da categoria).
/// </summary>
public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IRepository<Categoria> _categoriaRepository;

    public ProdutoService(IProdutoRepository produtoRepository, IRepository<Categoria> categoriaRepository)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IEnumerable<ProdutoResponse>> GetAllAsync()
    {
        var produtos = await _produtoRepository.GetAllComCategoriaAsync();
        return produtos.Select(ProdutoResponse.FromDomain);
    }

    public async Task<ProdutoResponse> GetByIdAsync(int id)
    {
        var produto = await _produtoRepository.GetByIdComCategoriaAsync(id)
            ?? throw new NotFoundException($"Produto com id {id} não foi encontrado.");

        return ProdutoResponse.FromDomain(produto);
    }

    public async Task<ProdutoResponse> CreateAsync(ProdutoRequest request)
    {
        var categoriaExiste = await _categoriaRepository.ExistsByIdAsync(request.CategoriaId);
        if (!categoriaExiste)
            throw new NotFoundException($"Categoria com id {request.CategoriaId} não foi encontrada.");

        var produto = new Produto
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Preco = request.Preco,
            Estoque = request.Estoque,
            CategoriaId = request.CategoriaId
        };

        await _produtoRepository.AddAsync(produto);
        await _produtoRepository.SaveChangesAsync();

        return ProdutoResponse.FromDomain(produto);
    }

    public async Task<PagedResponse<ProdutoResponse>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1)
            throw new DomainException("O parâmetro 'page' deve ser maior ou igual a 1.");

        if (pageSize is < 1 or > 100)
            throw new DomainException("O parâmetro 'pageSize' deve estar entre 1 e 100.");

        var (produtos, totalItems) = await _produtoRepository.GetPagedComCategoriaAsync(page, pageSize);

        var itens = produtos.Select(ProdutoResponse.FromDomain);

        return PagedResponse<ProdutoResponse>.Create(itens, page, pageSize, totalItems);
    }
}
