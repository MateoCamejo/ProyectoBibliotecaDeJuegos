using GameLibrary.Domain;
using GameLibrary.Domain.Enums;
using GameLibrary.DTOs;
using GameLibrary.Repositories.Interfaces;
using GameLibrary.Services.Exceptions;
using GameLibrary.Services.Interfaces;

namespace GameLibrary.Services.Implementations
{
    public class VideojuegoService : IVideojuegoService
    {
        private readonly IJuegoRepository _juegoRepo;
        private readonly IDesarrolladoraRepository _desarrolladoraRepo;
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly ICampanaService _campanaService;

        public VideojuegoService(
            IJuegoRepository juegoRepo,
            IDesarrolladoraRepository desarrolladoraRepo,
            ICategoriaRepository categoriaRepo,
            ICampanaService campanaService)
        {
            _juegoRepo = juegoRepo;
            _desarrolladoraRepo = desarrolladoraRepo;
            _categoriaRepo = categoriaRepo;
            _campanaService = campanaService;
        }

        public IEnumerable<JuegoResponseDto> GetAll(string? nombre = null)
        {
            var juegos = _juegoRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(nombre))
                juegos = juegos.Where(j => j.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

            return juegos.Select(ToDto);
        }

        public JuegoResponseDto GetById(int id) => ToDto(ObtenerJuegoOLanzarNotFound(id));

        public IEnumerable<JuegoResponseDto> GetByCategoria(int categoriaId)
        {
            if (_categoriaRepo.GetById(categoriaId) is null)
                throw new NotFoundException($"No existe la categoría con Id {categoriaId}.");

            return _juegoRepo.GetByCategoria(categoriaId).Select(ToDto);
        }

        public IEnumerable<JuegoResponseDto> GetByDesarrolladora(int desarrolladoraId)
        {
            if (_desarrolladoraRepo.GetById(desarrolladoraId) is null)
                throw new NotFoundException($"No existe la desarrolladora con Id {desarrolladoraId}.");

            return _juegoRepo.GetByDesarrolladora(desarrolladoraId).Select(ToDto);
        }

        public PrecioActualResponseDto GetPrecioActual(int id)
        {
            var juego = ObtenerJuegoOLanzarNotFound(id);
            var mejorCampana = _campanaService.ObtenerMejorPromocion(juego, DateTime.UtcNow);
            var precioFinal = _campanaService.CalcularPrecioFinal(juego);

            return new PrecioActualResponseDto
            {
                JuegoId = juego.Id,
                PrecioOriginal = juego.Precio,
                PrecioFinal = precioFinal,
                PorcentajeDescuento = mejorCampana?.PorcentajeDescuento ?? 0,
                CampanaAplicada = mejorCampana?.Nombre
            };
        }

        public JuegoResponseDto Create(JuegoCreateDto dto)
        {
            ValidarDatosBasicos(dto.Nombre, dto.Precio);

            if (_desarrolladoraRepo.GetById(dto.DesarrolladoraId) is null)
                throw new NotFoundException($"No existe la desarrolladora con Id {dto.DesarrolladoraId}.");

            var categorias = ResolverCategorias(dto.CategoriaIds);
            var estado = ParsearEstado(dto.Estado) ?? EstadoJuego.Proximamente;

            var juego = new Juego
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim() ?? string.Empty,
                Precio = dto.Precio,
                FechaLanzamiento = dto.FechaLanzamiento,
                DesarrolladoraId = dto.DesarrolladoraId,
                Categorias = categorias,
                Estado = estado
            };

            _juegoRepo.Add(juego);
            return ToDto(juego);
        }

        public JuegoResponseDto Update(int id, JuegoUpdateDto dto)
        {
            var juego = ObtenerJuegoOLanzarNotFound(id);
            ValidarDatosBasicos(dto.Nombre, dto.Precio);

            if (_desarrolladoraRepo.GetById(dto.DesarrolladoraId) is null)
                throw new NotFoundException($"No existe la desarrolladora con Id {dto.DesarrolladoraId}.");

            var estado = ParsearEstado(dto.Estado)
                ?? throw new BusinessRuleException(
                    $"Estado inválido '{dto.Estado}'. Valores permitidos: Disponible, Proximamente, Retirado.");

            juego.Nombre = dto.Nombre.Trim();
            juego.Descripcion = dto.Descripcion?.Trim() ?? string.Empty;
            juego.Precio = dto.Precio;
            juego.FechaLanzamiento = dto.FechaLanzamiento;
            juego.DesarrolladoraId = dto.DesarrolladoraId;
            juego.Categorias = ResolverCategorias(dto.CategoriaIds);
            juego.Estado = estado;

            _juegoRepo.Update(juego);
            return ToDto(juego);
        }

        private Juego ObtenerJuegoOLanzarNotFound(int id) =>
            _juegoRepo.GetById(id) ?? throw new NotFoundException($"No existe el videojuego con Id {id}.");

        private static void ValidarDatosBasicos(string nombre, decimal precio)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new BusinessRuleException("El nombre del videojuego es obligatorio.");

            if (precio < 0)
                throw new BusinessRuleException("El precio no puede ser negativo.");
        }

        private List<Categoria> ResolverCategorias(List<int> categoriaIds)
        {
            var categorias = new List<Categoria>();
            foreach (var categoriaId in categoriaIds.Distinct())
            {
                var categoria = _categoriaRepo.GetById(categoriaId)
                    ?? throw new NotFoundException($"No existe la categoría con Id {categoriaId}.");
                categorias.Add(categoria);
            }
            return categorias;
        }

        private static EstadoJuego? ParsearEstado(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return null;
            return Enum.TryParse<EstadoJuego>(estado, ignoreCase: true, out var resultado)
                ? resultado
                : null;
        }

        private JuegoResponseDto ToDto(Juego juego)
        {
            var desarrolladora = _desarrolladoraRepo.GetById(juego.DesarrolladoraId);

            return new JuegoResponseDto
            {
                Id = juego.Id,
                Nombre = juego.Nombre,
                Descripcion = juego.Descripcion,
                Precio = juego.Precio,
                FechaLanzamiento = juego.FechaLanzamiento,
                DesarrolladoraId = juego.DesarrolladoraId,
                DesarrolladoraNombre = desarrolladora?.Nombre ?? string.Empty,
                Categorias = juego.Categorias
                    .Select(c => new CategoriaResponseDto { Id = c.Id, Nombre = c.Nombre })
                    .ToList(),
                Estado = juego.Estado.ToString()
            };
        }
    }
}
