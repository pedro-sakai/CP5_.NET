using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EletronicosStore.API.Extensions;

/// <summary>
/// Gera um documento Swagger para CADA versão de API descoberta pelo ApiExplorer
/// (v1.0 e v2.0), em vez de um único documento fixo. A versão 1.0 recebe uma nota
/// explícita de que está deprecada.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "EletronicosStore API",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated
                    ? "⚠️ Esta versão está DEPRECADA. Use a v2 — o contrato de listagem de produtos muda " +
                      "de uma lista simples para um envelope paginado. API REST para gerenciamento de " +
                      "produtos, categorias, clientes e pedidos de uma loja de equipamentos eletrônicos."
                    : "API REST para gerenciamento de produtos, categorias, clientes e pedidos de uma " +
                      "loja de equipamentos eletrônicos. Nesta versão, a listagem de produtos é paginada."
            });
        }
    }
}
