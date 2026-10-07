using System.Reflection;
using System.Threading.RateLimiting;
using Asp.Versioning;
using EletronicosStore.API.Health;
using EletronicosStore.Application.Interfaces;
using EletronicosStore.Application.Interfaces.Services;
using EletronicosStore.Application.Services;
using EletronicosStore.Infrastructure.Persistence;
using EletronicosStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EletronicosStore.API.Extensions;

/// <summary>
/// Métodos de extensão para organizar o registro de serviços no Program.cs.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registra os repositórios: o genérico (para qualquer entidade) e os específicos por agregado.</summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        return services;
    }

    /// <summary>Registra os serviços de aplicação (regras de negócio).</summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProdutoService, ProdutoService>();
        return services;
    }

    /// <summary>
    /// Registra o versionamento de API (CP5): leitura da versão por query string
    /// (<c>?api-version=1.0</c>) ou header (<c>X-Api-Version</c>), com a v2.0 como padrão
    /// quando o cliente não informa nenhuma versão.
    /// </summary>
    public static IServiceCollection AddApiVersioningSupport(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    /// <summary>Configura o Swagger/OpenAPI (um documento por versão de API), incluindo os comentários XML.</summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });

        return services;
    }

    /// <summary>
    /// Configura os health checks: verificação do processo (self) e do banco de dados (Oracle,
    /// via o mesmo AppDbContext usado desde o CP2).
    /// </summary>
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("A API está em execução."), tags: new[] { "self" })
            .AddDbContextCheck<AppDbContext>("oracle_database", tags: new[] { "database" });

        return services;
    }

    /// <summary>
    /// Registra o rate limiting (CP5): política nomeada "fixed", janela fixa de 1 minuto,
    /// 10 requisições por IP. Como não há política global (sem AddPolicy padrão), somente os
    /// endpoints marcados com [EnableRateLimiting("fixed")] são limitados — o GET /health nunca
    /// passa pelo limitador, porque nunca foi marcado.
    /// </summary>
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Partição por IP: cada endereço remoto tem sua própria janela de 10 req/min.
            options.AddPolicy("fixed", httpContext => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.Headers.RetryAfter = "60";
                context.HttpContext.Response.Headers["X-RateLimit-Limit"] = "10";
                context.HttpContext.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Type = "about:blank",
                    Title = "Limite de requisições excedido",
                    Status = StatusCodes.Status429TooManyRequests,
                    Detail = "Você atingiu o limite de 10 requisições por minuto para este endpoint. Tente novamente em instantes.",
                    Instance = context.HttpContext.Request.Path
                };

                await context.HttpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            };
        });

        return services;
    }
}
