using GameLibrary.DAO;
using GameLibrary.Domain;
using GameLibrary.Domain.Enums;
using GameLibrary.DTOs;
using GameLibrary.Services.Exceptions;
using GameLibrary.Services.Interfaces;

namespace GameLibrary.Services.Implementations
{
    public class CompraService : ICompraService
    {
        private readonly ICompraDAO _compraDAO;
        private readonly IUsuarioDAO _usuarioDAO;
        private readonly IJuegoDAO _juegoDAO;
        private readonly ICampanaService _campanaService;

        public CompraService(
            ICompraDAO compraDAO,
            IUsuarioDAO usuarioDAO,
            IJuegoDAO juegoDAO,
            ICampanaService campanaService)
        {
            _compraDAO = compraDAO;
            _usuarioDAO = usuarioDAO;
            _juegoDAO = juegoDAO;
            _campanaService = campanaService;
        }

        // Crea la compra en estado Pendiente, ya validada y con el precio
        // final de cada juego "congelado" según las campañas activas en
        // este momento.
        public CompraResponseDto Create(CompraCreateDto dto)
        {
            var usuario = _usuarioDAO.BuscarPorId(dto.UsuarioId)
                ?? throw new NotFoundException($"No existe el usuario con Id {dto.UsuarioId}.");

            if (dto.JuegoIds is null || dto.JuegoIds.Count == 0)
                throw new BusinessRuleException("Debe indicar al menos un videojuego para comprar.");

            var juegoIds = dto.JuegoIds.Distinct().ToList();
            var detalles = new List<DetalleCompra>();

            foreach (var juegoId in juegoIds)
            {
                var juego = _juegoDAO.BuscarPorId(juegoId)
                    ?? throw new NotFoundException($"No existe el videojuego con Id {juegoId}.");

                ValidarJuegoComprable(juego, usuario);

                detalles.Add(new DetalleCompra
                {
                    JuegoId = juego.Id,
                    PrecioFinal = _campanaService.CalcularPrecioFinal(juego)
                });
            }

            var compra = new Compra
            {
                UsuarioId = usuario.Id,
                Fecha = DateTime.UtcNow,
                Detalles = detalles,
                ImporteFinal = detalles.Sum(d => d.PrecioFinal),
                Estado = EstadoCompra.Pendiente
            };

            _compraDAO.Guardar(compra);
            return ToDto(compra);
        }

        public CompraResponseDto GetById(int id) => ToDto(ObtenerCompraOLanzarNotFound(id));

        // "Antes de completar una compra" -> se vuelve a verificar todo acá,
        // por si algo cambió entre el Create (Pendiente) y la confirmación
        // (por ejemplo, el juego se retiró, o el usuario ya lo obtuvo por
        // otra vía). Recién acá se agrega a la biblioteca.
        public CompraResponseDto Confirmar(int id)
        {
            var compra = ObtenerCompraOLanzarNotFound(id);

            if (compra.Estado != EstadoCompra.Pendiente)
                throw new BusinessRuleException(
                    $"Solo se pueden confirmar compras en estado Pendiente (esta compra está en estado {compra.Estado}).");

            var usuario = _usuarioDAO.BuscarPorId(compra.UsuarioId)
                ?? throw new NotFoundException($"No existe el usuario con Id {compra.UsuarioId}.");

            foreach (var detalle in compra.Detalles)
            {
                var juego = _juegoDAO.BuscarPorId(detalle.JuegoId)
                    ?? throw new NotFoundException($"No existe el videojuego con Id {detalle.JuegoId}.");

                ValidarJuegoComprable(juego, usuario);
            }

            compra.Confirmar();

            foreach (var detalle in compra.Detalles)
            {
                _usuarioDAO.AgregarItemBiblioteca(usuario.Id, new ItemBiblioteca
                {
                    JuegoId = detalle.JuegoId,
                    FechaAdquisicion = DateTime.UtcNow,
                    HorasJugadas = 0,
                    UltimaVezUsado = null
                });
            }

            _compraDAO.ActualizarEstado(compra.Id, compra.Estado);

            return ToDto(compra);
        }

        // Una compra cancelada nunca debe tocar la biblioteca.
        public CompraResponseDto Cancelar(int id)
        {
            var compra = ObtenerCompraOLanzarNotFound(id);

            if (compra.Estado != EstadoCompra.Pendiente)
                throw new BusinessRuleException(
                    $"Solo se pueden cancelar compras en estado Pendiente (esta compra está en estado {compra.Estado}).");

            compra.Cancelar();
            _compraDAO.ActualizarEstado(compra.Id, compra.Estado);

            return ToDto(compra);
        }

        private static void ValidarJuegoComprable(Juego juego, Usuario usuario)
        {
            if (!juego.PuedeComprarse())
                throw new BusinessRuleException(
                    $"El videojuego '{juego.Nombre}' no está disponible para la venta (estado: {juego.Estado}).");

            if (usuario.Biblioteca.Contiene(juego.Id))
                throw new BusinessRuleException(
                    $"El usuario ya posee el videojuego '{juego.Nombre}' en su biblioteca.");
        }

        private Compra ObtenerCompraOLanzarNotFound(int id) =>
            _compraDAO.BuscarPorId(id) ?? throw new NotFoundException($"No existe la compra con Id {id}.");

        private CompraResponseDto ToDto(Compra compra) => new()
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
        };
    }
}
