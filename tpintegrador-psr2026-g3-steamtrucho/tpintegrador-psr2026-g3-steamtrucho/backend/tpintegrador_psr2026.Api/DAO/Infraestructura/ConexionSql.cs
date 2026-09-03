using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace tpintegrador_psr2026.Api.DAO.Infraestructura;

// Único lugar que conoce la cadena de conexión. Cada DAO recibe esta
// clase por inyección de dependencias y le pide una conexión nueva
// por operación (patrón clásico de ADO.NET: abrir, usar, cerrar).
public class ConexionSql
{
    private readonly string _connectionString;

    public ConexionSql(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "No se configuró la cadena de conexión 'Default' en appsettings.json.");
    }

    public SqlConnection CrearConexion() => new SqlConnection(_connectionString);
}
