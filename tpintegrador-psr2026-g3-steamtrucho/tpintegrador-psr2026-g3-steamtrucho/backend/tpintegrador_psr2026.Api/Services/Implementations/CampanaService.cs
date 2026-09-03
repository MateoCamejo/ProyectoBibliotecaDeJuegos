using tpintegrador_psr2026.Api.DAO;
using tpintegrador_psr2026.Api.Domain;
using tpintegrador_psr2026.Api.DTOs;
using tpintegrador_psr2026.Api.Services.Exceptions;
using tpintegrador_psr2026.Api.Services.Interfaces;

namespace tpintegrador_psr2026.Api.Services.Implementations;

public class CampanaService : ICampanaService
{
    private readonly ICampanaDAO _campanaDAO;
    private readonly ICategoriaDAO _categoriaDAO;
    private readonly IDesarrolladoraDAO _desarrolladoraDAO;

    public CampanaService(
        ICampanaDAO campanaDAO,
        ICategoriaDAO categoriaDAO,
        IDesarrolladoraDAO desarrolladoraDAO)
    {
        _campanaDAO = campanaDAO;
        _categoriaDAO = categoriaDAO;
        _desarrolladoraDAO = desarrolladoraDAO;
    }

    public IEnumerable<CampanaResponseDto> GetActivas()
    {
        var ahora = DateTime.UtcNow;
        return _campanaDAO.ListarActivas(ahora).Select(c => ToDto(c, ahora));
    }

    public CampanaResponseDto Create(CampanaCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new BusinessRuleException("El nombre de la campaña es obligatorio.");

        if (dto.FechaFin <= dto.FechaInicio)
            throw new BusinessRuleException("La fecha de fin debe ser posterior a la fecha de inicio.");

        if (dto.PorcentajeDescuento <= 0 || dto.PorcentajeDescuento > 100)
            throw new BusinessRuleException("El porcentaje de descuento debe estar entre 0 y 100.");

        if (dto.CategoriaId.HasValue && _categoriaDAO.BuscarPorId(dto.CategoriaId.Value) is null)
            throw new NotFoundException($"No existe la categoría con Id {dto.CategoriaId}.");

        if (dto.DesarrolladoraId.HasValue && _desarrolladoraDAO.BuscarPorId(dto.DesarrolladoraId.Value) is null)
            throw new NotFoundException($"No existe la desarrolladora con Id {dto.DesarrolladoraId}.");

        var sinCriterios = !dto.CategoriaId.HasValue
            && !dto.DesarrolladoraId.HasValue
            && (dto.JuegosAfectados is null || dto.JuegosAfectados.Count == 0);

        if (sinCriterios)
            throw new BusinessRuleException(
                "La campaña debe indicar al menos un criterio de alcance (categoría, desarrolladora o lista de juegos).");

        var campana = new Campana
        {
            Nombre = dto.Nombre.Trim(),
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            PorcentajeDescuento = dto.PorcentajeDescuento,
            CategoriaId = dto.CategoriaId,
            DesarrolladoraId = dto.DesarrolladoraId,
            JuegosAfectados = dto.JuegosAfectados
        };

        _campanaDAO.Guardar(campana);
        return ToDto(campana, DateTime.UtcNow);
    }

    public Campana? ObtenerMejorPromocion(Juego juego, DateTime fecha)
    {
        // Regla de negocio: los descuentos no son acumulables. Entre
        // todas las campañas activas que alcanzan al juego, se usa la
        // que ofrece el mayor porcentaje (la más beneficiosa).
        return _campanaDAO.ListarActivas(fecha)
            .Where(c => c.Alcanza(juego))
            .OrderByDescending(c => c.PorcentajeDescuento)
            .FirstOrDefault();
    }

    public decimal CalcularPrecioFinal(Juego juego)
    {
        var mejorCampana = ObtenerMejorPromocion(juego, DateTime.UtcNow);
        if (mejorCampana is null) return juego.Precio;

        var descuento = juego.Precio * (mejorCampana.PorcentajeDescuento / 100m);
        return Math.Round(juego.Precio - descuento, 2);
    }

    private static CampanaResponseDto ToDto(Campana c, DateTime fecha) => new()
    {
        Id = c.Id,
        Nombre = c.Nombre,
        FechaInicio = c.FechaInicio,
        FechaFin = c.FechaFin,
        PorcentajeDescuento = c.PorcentajeDescuento,
        CategoriaId = c.CategoriaId,
        DesarrolladoraId = c.DesarrolladoraId,
        JuegosAfectados = c.JuegosAfectados,
        Activa = c.EstaActiva(fecha)
    };
}
