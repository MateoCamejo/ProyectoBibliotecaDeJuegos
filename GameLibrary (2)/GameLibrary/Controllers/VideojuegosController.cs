using GameLibrary.DTOs;
using GameLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Controllers
{
    [ApiController]
    [Route("api/videojuegos")]
    public class VideojuegosController : ControllerBase
    {
        private readonly IVideojuegoService _videojuegoService;

        public VideojuegosController(IVideojuegoService videojuegoService)
        {
            _videojuegoService = videojuegoService;
        }

        /// <summary>Consulta el catálogo completo. Admite ?nombre= para buscar por texto.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<JuegoResponseDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<JuegoResponseDto>> GetAll([FromQuery] string? nombre)
        {
            return Ok(_videojuegoService.GetAll(nombre));
        }

        /// <summary>Consulta el detalle de un videojuego.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(JuegoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<JuegoResponseDto> GetById(int id)
        {
            return Ok(_videojuegoService.GetById(id));
        }

        /// <summary>Busca videojuegos por categoría.</summary>
        [HttpGet("categoria/{categoriaId:int}")]
        [ProducesResponseType(typeof(IEnumerable<JuegoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<JuegoResponseDto>> GetByCategoria(int categoriaId)
        {
            return Ok(_videojuegoService.GetByCategoria(categoriaId));
        }

        /// <summary>Busca videojuegos por desarrolladora.</summary>
        [HttpGet("desarrolladora/{desarrolladoraId:int}")]
        [ProducesResponseType(typeof(IEnumerable<JuegoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<JuegoResponseDto>> GetByDesarrolladora(int desarrolladoraId)
        {
            return Ok(_videojuegoService.GetByDesarrolladora(desarrolladoraId));
        }

        /// <summary>Precio actual de un juego, calculando la mejor promoción activa.</summary>
        [HttpGet("{id:int}/precio-actual")]
        [ProducesResponseType(typeof(PrecioActualResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PrecioActualResponseDto> GetPrecioActual(int id)
        {
            return Ok(_videojuegoService.GetPrecioActual(id));
        }

        /// <summary>Registra un nuevo videojuego.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(JuegoResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<JuegoResponseDto> Create([FromBody] JuegoCreateDto dto)
        {
            var creado = _videojuegoService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        /// <summary>Modifica un videojuego existente.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(JuegoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<JuegoResponseDto> Update(int id, [FromBody] JuegoUpdateDto dto)
        {
            return Ok(_videojuegoService.Update(id, dto));
        }
    }
}
