namespace EletronicosStore.Domain.Exceptions;

/// <summary>
/// Representa a ausência de um recurso solicitado (ex.: id inexistente).
/// Mapeada pelo GlobalExceptionHandler para HTTP 404 (Not Found).
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
