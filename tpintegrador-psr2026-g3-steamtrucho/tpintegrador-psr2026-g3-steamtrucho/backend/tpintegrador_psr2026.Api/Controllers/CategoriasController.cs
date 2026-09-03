using tpintegrador_psr2026.Api.DTOs;
using tpintegrador_psr2026.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace tpintegrador_psr2026.Api.Controllers;

// No forma parte de la lista mínima de Controllers del enunciado, pero
// es necesaria para poder dar de alta Categorías desde afuera y así
// poder crear Videojuegos y Campañas (que referencian CategoriaId)
// sin tocar datos "a mano".
[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    /// <summary>Lista todas las categorías.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<CategoriaResponseDto>> GetAll()
    {
        return Ok(_categoriaService.GetAll());
    }

    /// <summary>Registra una nueva categoría.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CategoriaResponseDto> Create([FromBody] CategoriaCreateDto dto)
    {
        var creada = _categoriaService.Create(dto);
        return CreatedAtAction(nameof(GetAll), null, creada);
    }
}
