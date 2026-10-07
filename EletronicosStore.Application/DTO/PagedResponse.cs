namespace EletronicosStore.Application.DTO;

/// <summary>
/// Envelope padrão para listagens paginadas (contrato v2 dos recursos que ganharam paginação).
/// </summary>
public record PagedResponse<T>(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    IEnumerable<T> Items)
{
    /// <summary>Indica se existe uma página anterior.</summary>
    public bool HasPrevious => Page > 1;

    /// <summary>Indica se existe uma próxima página.</summary>
    public bool HasNext => Page < TotalPages;

    public static PagedResponse<T> Create(IEnumerable<T> items, int page, int pageSize, int totalItems)
    {
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        return new PagedResponse<T>(page, pageSize, totalItems, totalPages, items);
    }
}
