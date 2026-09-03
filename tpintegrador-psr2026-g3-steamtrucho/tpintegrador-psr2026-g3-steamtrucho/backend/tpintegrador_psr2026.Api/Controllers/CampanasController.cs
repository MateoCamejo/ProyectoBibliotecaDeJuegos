using tpintegrador_psr2026.Api.DTOs;
using tpintegrador_psr2026.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace tpintegrador_psr2026.Api.Controllers;

[ApiController]
[Route("api/campanas")]
public class CampanasController : ControllerBase
{
    private readonly ICampanaService _campanaService;

    public CampanasController(ICampanaService campanaService)
    {
        _campanaService = campanaService;
    }

    /// <summary>Consulta las campañas comerciales activas en este momento.</summary>
    [HttpGet("activas")]
    [ProducesResponseType(typeof(IEnumerable<CampanaResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<CampanaResponseDto>> GetActivas()
    {
        return Ok(_campanaService.GetActivas());
    }

    /// <summary>
    /// Registra una campaña comercial (simula lo que en el futuro llegará
    /// por una API externa).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CampanaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CampanaResponseDto> Create([FromBody] CampanaCreateDto dto)
    {
        var creada = _campanaService.Create(dto);
        return CreatedAtAction(nameof(GetActivas), null, creada);
    }
}
