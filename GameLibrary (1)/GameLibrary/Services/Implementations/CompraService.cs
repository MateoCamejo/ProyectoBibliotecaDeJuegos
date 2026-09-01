using GameLibrary.Domain;
using GameLibrary.Domain.Enums;
using GameLibrary.DTOs;
using GameLibrary.Repositories.Interfaces;
using GameLibrary.Services.Exceptions;
using GameLibrary.Services.Interfaces;

namespace GameLibrary.Services.Implementations
{
    public class CompraService : ICompraService
    {
        private readonly ICompraRepository _compraRepo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IJuegoRepository _juegoRepo;
        private readonly ICampanaService _campanaService;

        public CompraService(
            ICompraRepository compraRepo,
            IUsuarioRepository usuarioRepo,
            IJuegoRepository juegoRepo,
            ICampanaService campanaService)
        {
            _compraRepo = compraRepo;
            _usuarioRepo = usuarioRepo;
            _juegoRepo = juegoRepo;
            _campanaService = campanaService;
        }

        // Crea la compra en estado Pendiente, ya validada y con el precio
        // final de cada juego "congelado" según las campañas activas en
        // este momento.
        public CompraResponseDto Create(CompraCreateDto dto)
        {
            var usuario = _usuarioRepo.GetById(dto.UsuarioId)
                ?? throw new NotFoundException($"No existe el usuario con Id {dto.UsuarioId}.");

            if (dto.JuegoIds is null || dto.JuegoIds.Count == 0)
                throw new BusinessRuleException("Debe indicar al menos un videojuego para comprar.");

            var juegoIds = dto.JuegoIds.Distinct().ToList();
            var detalles = new List<DetalleCompra>();

            foreach (var juegoId in juegoIds)
            {
                var juego = _juegoRepo.GetById(juegoId)
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

            _compraRepo.Add(compra);
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

            var usuario = _usuarioRepo.GetById(compra.UsuarioId)
                ?? throw new NotFoundException($"No existe el usuario con Id {compra.UsuarioId}.");

            foreach (var detalle in compra.Detalles)
            {
                var juego = _juegoRepo.GetById(detalle.JuegoId)
                    ?? throw new NotFoundException($"No existe el videojuego con Id {detalle.JuegoId}.");

                ValidarJuegoComprable(juego, usuario);
            }

            compra.Confirmar();

            foreach (var detalle in compra.Detalles)
                usuario.Biblioteca.AgregarJuego(detalle.JuegoId);

            _usuarioRepo.Update(usuario);
            _compraRepo.Update(compra);

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
            _compraRepo.Update(compra);

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
            _compraRepo.GetById(id) ?? throw new NotFoundException($"No existe la compra con Id {id}.");

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
                NombreJuego = _juegoRepo.GetById(d.JuegoId)?.Nombre ?? "(juego eliminado)",
                PrecioFinal = d.PrecioFinal
            }).ToList()
        };
    }
}
