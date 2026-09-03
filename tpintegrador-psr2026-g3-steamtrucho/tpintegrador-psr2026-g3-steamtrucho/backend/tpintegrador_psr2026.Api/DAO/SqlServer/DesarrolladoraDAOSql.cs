using tpintegrador_psr2026.Api.DAO.Infraestructura;
using tpintegrador_psr2026.Api.Domain;
using Microsoft.Data.SqlClient;

namespace tpintegrador_psr2026.Api.DAO.SqlServer;

public class DesarrolladoraDAOSql : IDesarrolladoraDAO
{
    private readonly ConexionSql _conexion;

    public DesarrolladoraDAOSql(ConexionSql conexion)
    {
        _conexion = conexion;
    }

    public List<Desarrolladora> ListarTodos()
    {
        const string sql = "SELECT Id, Nombre FROM Desarrolladoras ORDER BY Nombre";

        var desarrolladoras = new List<Desarrolladora>();

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
            desarrolladoras.Add(Mapear(lector));

        return desarrolladoras;
    }

    public Desarrolladora? BuscarPorId(int id)
    {
        const string sql = "SELECT Id, Nombre FROM Desarrolladoras WHERE Id = @Id";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);
        using var lector = comando.ExecuteReader();

        return lector.Read() ? Mapear(lector) : null;
    }

    public Desarrolladora Guardar(Desarrolladora desarrolladora)
    {
        const string sql = @"INSERT INTO Desarrolladoras (Nombre)
                              OUTPUT INSERTED.Id
                              VALUES (@Nombre)";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Nombre", desarrolladora.Nombre);

        desarrolladora.Id = Convert.ToInt32(comando.ExecuteScalar());
        return desarrolladora;
    }

    private static Desarrolladora Mapear(SqlDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        Nombre = lector.GetString(1)
    };
}
