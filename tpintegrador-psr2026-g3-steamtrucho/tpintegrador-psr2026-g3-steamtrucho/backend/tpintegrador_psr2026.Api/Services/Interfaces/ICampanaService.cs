using tpintegrador_psr2026.Api.Domain;
using tpintegrador_psr2026.Api.DTOs;

namespace tpintegrador_psr2026.Api.Services.Interfaces;

public interface ICampanaService
{
    IEnumerable<CampanaResponseDto> GetActivas();
    CampanaResponseDto Create(CampanaCreateDto dto);

    // Usados internamente por otros Services (Videojuego, Compra) para
    // resolver el precio real de un juego en un momento determinado.
    Campana? ObtenerMejorPromocion(Juego juego, DateTime fecha);
    decimal CalcularPrecioFinal(Juego juego);
}
