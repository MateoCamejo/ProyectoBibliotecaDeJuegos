using tpintegrador_psr2026.Api.DTOs;
using tpintegrador_psr2026.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace tpintegrador_psr2026.Api.Controllers;

// Idem CategoriasController: soporte necesario para poder crear
// Videojuegos y Campañas de punta a punta desde Swagger.
[ApiController]
[Route("api/desarrolladoras")]
public class DesarrolladorasController : ControllerBase
{
    private readonly IDesarrolladoraService _desarrolladoraService;

    public DesarrolladorasController(IDesarrolladoraService desarrolladoraService)
    {
        _desarrolladoraService = desarrolladoraService;
    }

    /// <summary>Lista todas las desarrolladoras.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DesarrolladoraResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DesarrolladoraResponseDto>> GetAll()
    {
        return Ok(_desarrolladoraService.GetAll());
    }

    /// <summary>Registra una nueva desarrolladora.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(DesarrolladoraResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DesarrolladoraResponseDto> Create([FromBody] DesarrolladoraCreateDto dto)
    {
        var creada = _desarrolladoraService.Create(dto);
        return CreatedAtAction(nameof(GetAll), null, creada);
    }
}
