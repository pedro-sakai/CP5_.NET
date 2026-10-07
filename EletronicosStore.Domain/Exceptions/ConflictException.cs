namespace EletronicosStore.Domain.Exceptions;

/// <summary>
/// Representa um conflito de estado (ex.: registro duplicado, violação de unicidade).
/// Mapeada pelo GlobalExceptionHandler para HTTP 409 (Conflict).
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
