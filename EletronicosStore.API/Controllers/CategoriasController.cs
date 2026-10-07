using Asp.Versioning;
using EletronicosStore.Application.DTO;
using EletronicosStore.Application.Interfaces;
using EletronicosStore.Domain.Entities;
using EletronicosStore.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EletronicosStore.API.Controllers;

/// <summary>
/// Operações de consulta e cadastro de categorias.
/// Demonstra o uso direto do repositório genérico (IRepository&lt;Categoria&gt;).
/// Rota base: /api/categorias
/// </summary>
[ApiController]
[ApiVersionNeutral]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IRepository<Categoria> _categoriaRepository;

    public CategoriasController(IRepository<Categoria> categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    /// <summary>Lista todas as categorias cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var categorias = await _categoriaRepository.GetAllAsync();
        return Ok(categorias.Select(CategoriaResponse.FromDomain));
    }

    /// <summary>Busca uma categoria pelo id.</summary>
    /// <response code="404">Categoria não encontrada.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Categoria com id {id} não foi encontrada.");

        return Ok(CategoriaResponse.FromDomain(categoria));
    }

    /// <summary>Cadastra uma nova categoria.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] CategoriaRequest request)
    {
        var categoria = request.ToDomain();

        await _categoriaRepository.AddAsync(categoria);
        await _categoriaRepository.SaveChangesAsync();

        var response = CategoriaResponse.FromDomain(categoria);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, response);
    }
}
