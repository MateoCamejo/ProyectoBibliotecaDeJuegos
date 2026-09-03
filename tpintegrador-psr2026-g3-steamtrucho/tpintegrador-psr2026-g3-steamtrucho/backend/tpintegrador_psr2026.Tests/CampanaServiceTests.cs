using tpintegrador_psr2026.Api.Domain;
using tpintegrador_psr2026.Api.Domain.Enums;
using tpintegrador_psr2026.Api.Services.Implementations;
using tpintegrador_psr2026.Tests.Fakes;
using Xunit;

namespace tpintegrador_psr2026.Tests;

public class CampanaServiceTests
{
    private static Juego CrearJuego(int id, decimal precio, int categoriaId, int desarrolladoraId) => new()
    {
        Id = id,
        Nombre = "Juego de prueba",
        Precio = precio,
        DesarrolladoraId = desarrolladoraId,
        Categorias = new List<Categoria> { new() { Id = categoriaId, Nombre = "Acción" } },
        Estado = EstadoJuego.Disponible
    };

    [Fact]
    public void ObtenerMejorPromocion_ConVariasCampanasActivas_DevuelveLaDeMayorDescuento()
    {
        var ahora = DateTime.UtcNow;
        var juego = CrearJuego(id: 1, precio: 1000m, categoriaId: 5, desarrolladoraId: 9);

        var campanas = new List<Campana>
        {
            new() { Id = 1, Nombre = "Descuento chico", FechaInicio = ahora.AddDays(-1), FechaFin = ahora.AddDays(1), PorcentajeDescuento = 10m, CategoriaId = 5 },
            new() { Id = 2, Nombre = "Descuento grande", FechaInicio = ahora.AddDays(-1), FechaFin = ahora.AddDays(1), PorcentajeDescuento = 30m, DesarrolladoraId = 9 },
            new() { Id = 3, Nombre = "Vencida (no debería contar)", FechaInicio = ahora.AddDays(-10), FechaFin = ahora.AddDays(-5), PorcentajeDescuento = 90m, CategoriaId = 5 },
        };

        var service = new CampanaService(new FakeCampanaDAO(campanas), new FakeCategoriaDAO(), new FakeDesarrolladoraDAO());

        var mejor = service.ObtenerMejorPromocion(juego, ahora);

        Assert.NotNull(mejor);
        Assert.Equal("Descuento grande", mejor!.Nombre);
    }

    [Fact]
    public void CalcularPrecioFinal_SinCampanasActivas_DevuelveElPrecioOriginal()
    {
        var juego = CrearJuego(id: 2, precio: 500m, categoriaId: 1, desarrolladoraId: 1);
        var service = new CampanaService(new FakeCampanaDAO(new List<Campana>()), new FakeCategoriaDAO(), new FakeDesarrolladoraDAO());

        var precioFinal = service.CalcularPrecioFinal(juego);

        Assert.Equal(500m, precioFinal);
    }

    [Fact]
    public void CalcularPrecioFinal_ConCampanaActiva_AplicaElDescuentoCorrectamente()
    {
        var ahora = DateTime.UtcNow;
        var juego = CrearJuego(id: 3, precio: 1000m, categoriaId: 7, desarrolladoraId: 1);

        var campanas = new List<Campana>
        {
            new() { Id = 1, Nombre = "Festival", FechaInicio = ahora.AddHours(-1), FechaFin = ahora.AddHours(1), PorcentajeDescuento = 25m, CategoriaId = 7 }
        };

        var service = new CampanaService(new FakeCampanaDAO(campanas), new FakeCategoriaDAO(), new FakeDesarrolladoraDAO());

        var precioFinal = service.CalcularPrecioFinal(juego);

        Assert.Equal(750m, precioFinal);
    }
}
