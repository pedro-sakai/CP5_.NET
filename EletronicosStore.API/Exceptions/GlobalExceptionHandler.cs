using EletronicosStore.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EletronicosStore.API.Exceptions;

/// <summary>
/// Handler central de exceções da API. Converte qualquer exceção não tratada em uma
/// resposta padronizada no formato RFC 7807 (ProblemDetails).
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "Exceção não tratada: {Message}. TraceId: {TraceId}",
            exception.Message,
            traceId);

        var (statusCode, title, detail) = MapException(exception);

        var problem = new ProblemDetails
        {
            Type = "about:blank",
            Title = title,
            Status = statusCode,
            Detail = _environment.IsDevelopment() ? detail : "Ocorreu um erro ao processar a requisição.",
            Instance = httpContext.Request.Path
        };

        // Em Development, expõe o traceId no corpo do erro para facilitar a depuração.
        if (_environment.IsDevelopment())
        {
            problem.Extensions["traceId"] = traceId;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, "Conflito de dados", e.Message),
            DomainException e => (StatusCodes.Status400BadRequest, "Não foi possível concluir a operação", e.Message),
            ArgumentNullException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            ArgumentException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            KeyNotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, "Não foi possível concluir a operação", e.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro interno do servidor",
                "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };
    }
}
