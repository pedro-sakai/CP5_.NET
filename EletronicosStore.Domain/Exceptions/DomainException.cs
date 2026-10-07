namespace EletronicosStore.Domain.Exceptions;

/// <summary>
/// Representa a violação de uma regra de negócio do domínio.
/// Mapeada pelo GlobalExceptionHandler para HTTP 400 (Bad Request).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
