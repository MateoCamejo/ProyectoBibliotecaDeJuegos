using GameLibrary.Domain;
using GameLibrary.DTOs;
using GameLibrary.Repositories.Interfaces;
using GameLibrary.Services.Exceptions;
using GameLibrary.Services.Interfaces;

namespace GameLibrary.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IJuegoRepository _juegoRepo;
        private readonly ICompraRepository _compraRepo;

        public UsuarioService(
            IUsuarioRepository usuarioRepo,
            IJuegoRepository juegoRepo,
            ICompraRepository compraRepo)
        {
            _usuarioRepo = usuarioRepo;
            _juegoRepo = juegoRepo;
            _compraRepo = compraRepo;
        }

        public UsuarioResponseDto Create(UsuarioCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new BusinessRuleException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BusinessRuleException("El email es obligatorio.");

            if (_usuarioRepo.GetByEmail(dto.Email) is not null)
                throw new BusinessRuleException($"Ya existe un usuario registrado con el email '{dto.Email}'.");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Email = dto.Email.Trim()
            };

            _usuarioRepo.Add(usuario);

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
                    NombreJuego = _juegoRepo.GetById(item.JuegoId)?.Nombre ?? "(juego eliminado)",
                    FechaAdquisicion = item.FechaAdquisicion,
                    HorasJugadas = item.HorasJugadas,
                    UltimaVezUsado = item.UltimaVezUsado
                }).ToList()
            };
        }

        public IEnumerable<CompraResponseDto> GetCompras(int usuarioId)
        {
            ObtenerUsuarioOLanzarNotFound(usuarioId);

            return _compraRepo.GetByUsuarioId(usuarioId).Select(compra => new CompraResponseDto
            {
                Id = compra.Id,
                UsuarioId = compra.UsuarioId,
                Fecha = compra.Fecha,
                ImporteFinal = compra.ImporteFinal,
                Estado = compra.Estado.ToString(),
                Detalles = compra.Detalles.Select(d => new DetalleCompraResponseDto
                {
                    JuegoId = d.JuegoId,
                    NombreJuego = _juegoRepo.GetById(d.JuegoId)?.Nombre ?? "(juego eliminado)",
                    PrecioFinal = d.PrecioFinal
                }).ToList()
            });
        }

        private Usuario ObtenerUsuarioOLanzarNotFound(int usuarioId) =>
            _usuarioRepo.GetById(usuarioId)
                ?? throw new NotFoundException($"No existe el usuario con Id {usuarioId}.");
    }
}
