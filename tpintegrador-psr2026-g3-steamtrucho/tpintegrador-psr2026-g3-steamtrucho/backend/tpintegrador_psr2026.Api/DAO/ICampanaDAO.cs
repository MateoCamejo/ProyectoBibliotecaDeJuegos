using tpintegrador_psr2026.Api.Domain;

namespace tpintegrador_psr2026.Api.DAO;

public interface ICampanaDAO
{
    List<Campana> ListarActivas(DateTime fecha);
    Campana Guardar(Campana campana);
}
