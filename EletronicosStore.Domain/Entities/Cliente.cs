using EletronicosStore.Domain.Common;

namespace EletronicosStore.Domain.Entities;

public class Cliente : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
