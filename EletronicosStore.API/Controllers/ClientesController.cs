using Asp.Versioning;
using EletronicosStore.Application.DTO;
using EletronicosStore.Application.Interfaces;
using EletronicosStore.Domain.Entities;
using EletronicosStore.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EletronicosStore.API.Controllers;

/// <summary>
/// Operações de consulta e cadastro de clientes.
/// Rota base: /api/clientes
/// </summary>
[ApiController]
[ApiVersionNeutral]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IRepository<Cliente> _clienteRepository;

    public ClientesController(IRepository<Cliente> clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    /// <summary>Lista todos os clientes cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        return Ok(clientes.Select(ClienteResponse.FromDomain));
    }

    /// <summary>Busca um cliente pelo id.</summary>
    /// <response code="404">Cliente não encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Cliente com id {id} não foi encontrado.");

        return Ok(ClienteResponse.FromDomain(cliente));
    }

    /// <summary>Cadastra um novo cliente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] ClienteRequest request)
    {
        var cliente = request.ToDomain();

        await _clienteRepository.AddAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        var response = ClienteResponse.FromDomain(cliente);
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, response);
    }
}
