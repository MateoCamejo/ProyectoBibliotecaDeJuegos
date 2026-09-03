using tpintegrador_psr2026.Api.Domain;
using tpintegrador_psr2026.Api.Domain.Enums;
using Xunit;

namespace tpintegrador_psr2026.Tests;

public class JuegoTests
{
    [Theory]
    [InlineData(EstadoJuego.Disponible, true)]
    [InlineData(EstadoJuego.Proximamente, false)]
    [InlineData(EstadoJuego.Retirado, false)]
    public void PuedeComprarse_SoloCuandoEstaDisponible(EstadoJuego estado, bool esperado)
    {
        var juego = new Juego { Estado = estado };

        Assert.Equal(esperado, juego.PuedeComprarse());
    }
}

public class CampanaTests
{
    [Fact]
    public void EstaActiva_DentroDelRangoDeFechas_DevuelveTrue()
    {
        var ahora = DateTime.UtcNow;
        var campana = new Campana { FechaInicio = ahora.AddDays(-1), FechaFin = ahora.AddDays(1) };

        Assert.True(campana.EstaActiva(ahora));
    }

    [Fact]
    public void EstaActiva_FueraDelRangoDeFechas_DevuelveFalse()
    {
        var ahora = DateTime.UtcNow;
        var campana = new Campana { FechaInicio = ahora.AddDays(-10), FechaFin = ahora.AddDays(-5) };

        Assert.False(campana.EstaActiva(ahora));
    }

    [Fact]
    public void Alcanza_PorCategoria_DevuelveTrueSiElJuegoLaTiene()
    {
        var categoria = new Categoria { Id = 3, Nombre = "RPG" };
        var juego = new Juego { Categorias = new List<Categoria> { categoria } };
        var campana = new Campana { CategoriaId = 3 };

        Assert.True(campana.Alcanza(juego));
    }

    [Fact]
    public void Alcanza_SinNingunCriterioQueCoincida_DevuelveFalse()
    {
        var juego = new Juego { Id = 1, DesarrolladoraId = 1, Categorias = new List<Categoria>() };
        var campana = new Campana { CategoriaId = 99, DesarrolladoraId = 99, JuegosAfectados = new List<int> { 55 } };

        Assert.False(campana.Alcanza(juego));
    }
}

public class BibliotecaTests
{
    [Fact]
    public void AgregarJuego_LoIncorporaALaBiblioteca()
    {
        var biblioteca = new Biblioteca();

        biblioteca.AgregarJuego(juegoId: 7);

        Assert.True(biblioteca.Contiene(7));
        Assert.Single(biblioteca.Items);
    }

    [Fact]
    public void AgregarJuego_SiYaLoTiene_NoLoDuplica()
    {
        var biblioteca = new Biblioteca();

        biblioteca.AgregarJuego(juegoId: 7);
        biblioteca.AgregarJuego(juegoId: 7);

        Assert.Single(biblioteca.Items);
    }
}
