using Asp.Versioning.ApiExplorer;
using Oracle.EntityFrameworkCore.Infrastructure;
using EletronicosStore.API.Exceptions;
using EletronicosStore.API.Extensions;
using EletronicosStore.API.Health;
using EletronicosStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString, oracleOptions =>
    {
        oracleOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);
    }));

builder.Services.AddControllers();

// Repositórios (genérico + específicos) e serviços de aplicação
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

// Tratamento global de exceções (RFC 7807 / ProblemDetails)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Versionamento de API (CP5): v1.0 deprecada / v2.0 atual, via query string ou header
builder.Services.AddApiVersioningSupport();

// Swagger / OpenAPI (um documento por versão)
builder.Services.AddSwaggerDocumentation();

// Health checks (self + banco Oracle)
builder.Services.AddCustomHealthChecks();

// Rate limiting (CP5): política "fixed", aplicada só nos endpoints marcados com [EnableRateLimiting]
builder.Services.AddCustomRateLimiting();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Um grupo por versão de API descoberta (v1, v2, ...), para o seletor do Swagger UI.
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            var nome = description.IsDeprecated
                ? $"EletronicosStore API {description.GroupName} (DEPRECADA)"
                : $"EletronicosStore API {description.GroupName}";

            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", nome);
        }

        options.RoutePrefix = "swagger";
    });
}

// Rate limiting entra depois do exception handler e antes dos controllers, como pede o CP5.
app.UseRateLimiter();

app.UseAuthorization();
app.MapControllers();

// Único endpoint de health check, reunindo todos os checks registrados (self + banco).
// Nunca é decorado com [EnableRateLimiting], então o teto de requisições nunca se aplica a ele.
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.Run();

// Necessário para expor a classe Program aos projetos de teste (WebApplicationFactory / testes futuros).
public partial class Program { }
