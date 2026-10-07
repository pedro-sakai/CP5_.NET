using Asp.Versioning;
using EletronicosStore.Application.DTO;
using EletronicosStore.Application.Interfaces;
using EletronicosStore.Domain.Entities;
using EletronicosStore.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EletronicosStore.API.Controllers;

/// <summary>
/// Operações de consulta e criação de pedidos.
/// Rota base: /api/pedidos
/// </summary>
[ApiController]
[ApiVersionNeutral]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IRepository<Cliente> _clienteRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(
        IPedidoRepository pedidoRepository,
        IRepository<Cliente> clienteRepository,
        IProdutoRepository produtoRepository,
        ILogger<PedidosController> logger)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _produtoRepository = produtoRepository;
        _logger = logger;
    }

    /// <summary>Lista todos os pedidos, com cliente e itens já carregados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PedidoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var pedidos = await _pedidoRepository.GetAllComDetalhesAsync();
        return Ok(pedidos.Select(PedidoResponse.FromDomain));
    }

    /// <summary>Busca um pedido pelo id.</summary>
    /// <response code="404">Pedido não encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var pedido = await _pedidoRepository.GetByIdComDetalhesAsync(id)
            ?? throw new NotFoundException($"Pedido com id {id} não foi encontrado.");

        return Ok(PedidoResponse.FromDomain(pedido));
    }

    /// <summary>
    /// Cria um novo pedido para um cliente existente, a partir de uma lista de itens
    /// (produto + quantidade). O preço unitário é obtido do cadastro do produto no momento da compra.
    /// </summary>
    /// <response code="404">Cliente ou produto informado não existe.</response>
    /// <response code="400">Quantidade inválida em algum item.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] PedidoRequest request)
    {
        var traceId = HttpContext.TraceIdentifier;

        _logger.LogInformation(
            "Iniciando criação de pedido para o cliente {ClienteId} com {QuantidadeItens} item(ns). TraceId: {TraceId}",
            request.ClienteId, request.Itens.Count, traceId);

        var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId)
            ?? throw new NotFoundException($"Cliente com id {request.ClienteId} não foi encontrado.");

        var pedido = new Pedido
        {
            ClienteId = cliente.Id,
            DataPedido = DateTime.Now
        };

        foreach (var itemRequest in request.Itens)
        {
            var produto = await _produtoRepository.GetByIdAsync(itemRequest.ProdutoId)
                ?? throw new NotFoundException($"Produto com id {itemRequest.ProdutoId} não foi encontrado.");

            pedido.AdicionarItem(produto.Id, itemRequest.Quantidade, produto.Preco);
        }

        await _pedidoRepository.AddAsync(pedido);
        await _pedidoRepository.SaveChangesAsync();

        var pedidoCriado = await _pedidoRepository.GetByIdComDetalhesAsync(pedido.Id);

        _logger.LogInformation(
            "Pedido {PedidoId} criado com sucesso para o cliente {ClienteId}. Valor total: {ValorTotal}. TraceId: {TraceId}",
            pedido.Id, cliente.Id, pedido.ValorTotal, traceId);

        return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, PedidoResponse.FromDomain(pedidoCriado!));
    }
}
