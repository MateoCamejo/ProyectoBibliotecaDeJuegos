using GameLibrary.DTOs;
using GameLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Controllers
{
    [ApiController]
    [Route("api/compras")]
    public class ComprasController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public ComprasController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        /// <summary>
        /// Inicia una compra: verifica disponibilidad, que el usuario no
        /// posea ya el juego, y calcula el precio aplicando descuentos
        /// activos. Queda en estado Pendiente hasta confirmarla.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CompraResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CompraResponseDto> Create([FromBody] CompraCreateDto dto)
        {
            var creada = _compraService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
        }

        /// <summary>Consulta el detalle de una compra.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CompraResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CompraResponseDto> GetById(int id)
        {
            return Ok(_compraService.GetById(id));
        }

        /// <summary>Confirma una compra pendiente y la agrega a la biblioteca del usuario.</summary>
        [HttpPatch("{id:int}/confirmar")]
        [ProducesResponseType(typeof(CompraResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CompraResponseDto> Confirmar(int id)
        {
            return Ok(_compraService.Confirmar(id));
        }

        /// <summary>Cancela una compra pendiente (no modifica la biblioteca).</summary>
        [HttpPatch("{id:int}/cancelar")]
        [ProducesResponseType(typeof(CompraResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CompraResponseDto> Cancelar(int id)
        {
            return Ok(_compraService.Cancelar(id));
        }
    }
}
