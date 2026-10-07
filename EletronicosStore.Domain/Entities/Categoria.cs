using EletronicosStore.Domain.Common;

namespace EletronicosStore.Domain.Entities;

public class Categoria : BaseEntity
{
    public string Nome { get; set; } = string.Empty;

    public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}
