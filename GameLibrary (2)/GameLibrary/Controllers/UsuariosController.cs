using GameLibrary.DTOs;
using GameLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>Registra un usuario.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UsuarioResponseDto> Create([FromBody] UsuarioCreateDto dto)
        {
            var creado = _usuarioService.Create(dto);
            return CreatedAtAction(nameof(GetBiblioteca), new { id = creado.Id }, creado);
        }

        /// <summary>Consulta la biblioteca personal de un usuario.</summary>
        [HttpGet("{id:int}/biblioteca")]
        [ProducesResponseType(typeof(BibliotecaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BibliotecaResponseDto> GetBiblioteca(int id)
        {
            return Ok(_usuarioService.GetBiblioteca(id));
        }

        /// <summary>Consulta el historial de compras de un usuario.</summary>
        [HttpGet("{id:int}/compras")]
        [ProducesResponseType(typeof(IEnumerable<CompraResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<CompraResponseDto>> GetCompras(int id)
        {
            return Ok(_usuarioService.GetCompras(id));
        }
    }
}
