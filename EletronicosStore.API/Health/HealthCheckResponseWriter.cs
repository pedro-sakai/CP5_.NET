using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EletronicosStore.API.Health;

/// <summary>
/// Escreve o relatório de health check em um JSON legível, tanto para humanos
/// quanto para ferramentas de monitoramento.
/// </summary>
public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public static Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var isDevelopment = context.RequestServices
            .GetRequiredService<IHostEnvironment>()
            .IsDevelopment();

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                durationMs = Math.Round(entry.Value.Duration.TotalMilliseconds, 2),
                description = entry.Value.Description,
                error = isDevelopment ? entry.Value.Exception?.Message : null
            })
        };

        var json = JsonSerializer.Serialize(payload, SerializerOptions);
        return context.Response.WriteAsync(json);
    }
}
