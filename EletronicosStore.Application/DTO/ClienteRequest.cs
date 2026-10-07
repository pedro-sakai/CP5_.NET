using EletronicosStore.Domain.Entities;

namespace EletronicosStore.Application.DTO;

/// <summary>DTO de entrada para criação/atualização de um cliente.</summary>
public class ClienteRequest
{
    /// <summary>Nome completo do cliente.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail de contato.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>CPF (somente números).</summary>
    public string CPF { get; set; } = string.Empty;

    /// <summary>Converte o DTO para a entidade de domínio <see cref="Cliente"/>.</summary>
    public Cliente ToDomain() => new() { Nome = Nome, Email = Email, CPF = CPF };
}
