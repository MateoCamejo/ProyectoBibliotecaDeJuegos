using GameLibrary.DAO;
using GameLibrary.Domain;
using GameLibrary.DTOs;
using GameLibrary.Services.Exceptions;
using GameLibrary.Services.Interfaces;

namespace GameLibrary.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioDAO _usuarioDAO;
        private readonly IJuegoDAO _juegoDAO;
        private readonly ICompraDAO _compraDAO;

        public UsuarioService(
            IUsuarioDAO usuarioDAO,
            IJuegoDAO juegoDAO,
            ICompraDAO compraDAO)
        {
            _usuarioDAO = usuarioDAO;
            _juegoDAO = juegoDAO;
            _compraDAO = compraDAO;
        }

        public UsuarioResponseDto Create(UsuarioCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new BusinessRuleException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BusinessRuleException("El email es obligatorio.");

            if (_usuarioDAO.BuscarPorEmail(dto.Email) is not null)
                throw new BusinessRuleException($"Ya existe un usuario registrado con el email '{dto.Email}'.");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Email = dto.Email.Trim()
            };

            _usuarioDAO.Guardar(usuario);

            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        public BibliotecaResponseDto GetBiblioteca(int usuarioId)
        {
            var usuario = ObtenerUsuarioOLanzarNotFound(usuarioId);

            return new BibliotecaResponseDto
            {
                UsuarioId = usuario.Id,
                Items = usuario.Biblioteca.Items.Select(item => new ItemBibliotecaResponseDto
                {
                    JuegoId = item.JuegoId,
                    NombreJuego = _juegoDAO.BuscarPorId(item.JuegoId)?.Nombre ?? "(juego eliminado)",
                    FechaAdquisicion = item.FechaAdquisicion,
                    HorasJugadas = item.HorasJugadas,
                    UltimaVezUsado = item.UltimaVezUsado
                }).ToList()
            };
        }

        public IEnumerable<CompraResponseDto> GetCompras(int usuarioId)
        {
            ObtenerUsuarioOLanzarNotFound(usuarioId);

            return _compraDAO.ListarPorUsuario(usuarioId).Select(compra => new CompraResponseDto
            {
                Id = compra.Id,
                UsuarioId = compra.UsuarioId,
                Fecha = compra.Fecha,
                ImporteFinal = compra.ImporteFinal,
                Estado = compra.Estado.ToString(),
                Detalles = compra.Detalles.Select(d => new DetalleCompraResponseDto
                {
                    JuegoId = d.JuegoId,
                    NombreJuego = _juegoDAO.BuscarPorId(d.JuegoId)?.Nombre ?? "(juego eliminado)",
                    PrecioFinal = d.PrecioFinal
                }).ToList()
            });
        }

        private Usuario ObtenerUsuarioOLanzarNotFound(int usuarioId) =>
            _usuarioDAO.BuscarPorId(usuarioId)
                ?? throw new NotFoundException($"No existe el usuario con Id {usuarioId}.");
    }
}
