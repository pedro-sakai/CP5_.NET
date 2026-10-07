using EletronicosStore.Application.DTO;

namespace EletronicosStore.Application.Interfaces.Services;

/// <summary>Regras de negócio para o agregado Produto.</summary>
public interface IProdutoService
{
    Task<IEnumerable<ProdutoResponse>> GetAllAsync();
    Task<ProdutoResponse> GetByIdAsync(int id);
    Task<ProdutoResponse> CreateAsync(ProdutoRequest request);

    /// <summary>
    /// Retorna uma página de produtos (contrato v2). Valida o intervalo de <paramref name="page"/>
    /// e <paramref name="pageSize"/> antes de consultar o repositório.
    /// </summary>
    /// <exception cref="EletronicosStore.Domain.Exceptions.DomainException">
    /// Lançada quando page &lt; 1 ou pageSize está fora do intervalo de 1 a 100.
    /// </exception>
    Task<PagedResponse<ProdutoResponse>> GetPagedAsync(int page, int pageSize);
}
