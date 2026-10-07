using EletronicosStore.Domain.Entities;

namespace EletronicosStore.Application.DTO;

/// <summary>DTO de saída para cliente.</summary>
public record ClienteResponse(int Id, string Nome, string Email, string CPF)
{
    public static ClienteResponse FromDomain(Cliente cliente) =>
        new(cliente.Id, cliente.Nome, cliente.Email, cliente.CPF);
}
