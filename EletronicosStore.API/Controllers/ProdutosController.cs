using Asp.Versioning;
using EletronicosStore.Application.DTO;
using EletronicosStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EletronicosStore.API.Controllers;

/// <summary>
/// Operações de consulta e cadastro de produtos.
/// Rota base: /api/produtos
/// </summary>
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IProdutoService produtoService, ILogger<ProdutosController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    /// <summary>
    /// [DEPRECADO] Lista todos os produtos cadastrados, com a categoria já preenchida.
    /// Contrato antigo (CP3): devolve o array completo, sem paginação. Use a v2.
    /// </summary>
    /// <response code="200">Lista completa de produtos.</response>
    [HttpGet, MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllV1()
    {
        var produtos = await _produtoService.GetAllAsync();
        return Ok(produtos);
    }

    /// <summary>
    /// Lista os produtos cadastrados de forma paginada (envelope com totais).
    /// </summary>
    /// <param name="page">Página solicitada (mínimo 1, padrão 1).</param>
    /// <param name="pageSize">Tamanho da página (de 1 a 100, padrão 20).</param>
    /// <response code="200">Página de produtos, mesmo que vazia (page além do total).</response>
    /// <response code="400">page ou pageSize fora do intervalo permitido.</response>
    [HttpGet, MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(PagedResponse<ProdutoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllV2([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var pagina = await _produtoService.GetPagedAsync(page, pageSize);
        return Ok(pagina);
    }

    /// <summary>Busca um produto pelo id. Disponível nas versões 1.0 e 2.0.</summary>
    /// <param name="id">Id do produto.</param>
    /// <response code="404">Produto não encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var produto = await _produtoService.GetByIdAsync(id);
        return Ok(produto);
    }

    /// <summary>
    /// Cadastra um novo produto. Disponível nas versões 1.0 e 2.0.
    /// Limitado a 10 requisições por minuto por IP (ver CP5 — rate limiting).
    /// </summary>
    /// <remarks>A categoria informada (CategoriaId) precisa existir previamente.</remarks>
    /// <response code="400">Categoria inexistente ou dados inválidos.</response>
    /// <response code="429">Limite de requisições excedido (10 por minuto por IP).</response>
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Post([FromBody] ProdutoRequest request)
    {
        var traceId = HttpContext.TraceIdentifier;

        _logger.LogInformation(
            "Iniciando criação de produto {ProdutoNome} na categoria {CategoriaId}. TraceId: {TraceId}",
            request.Nome, request.CategoriaId, traceId);

        var produtoCriado = await _produtoService.CreateAsync(request);

        _logger.LogInformation(
            "Produto {ProdutoId} ({ProdutoNome}) criado com sucesso. TraceId: {TraceId}",
            produtoCriado.Id, produtoCriado.Nome, traceId);

        return CreatedAtAction(nameof(GetById), new { id = produtoCriado.Id }, produtoCriado);
    }
}
