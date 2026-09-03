using tpintegrador_psr2026.Api.DAO.Infraestructura;
using tpintegrador_psr2026.Api.Domain;
using Microsoft.Data.SqlClient;

namespace tpintegrador_psr2026.Api.DAO.SqlServer;

public class CategoriaDAOSql : ICategoriaDAO
{
    private readonly ConexionSql _conexion;

    public CategoriaDAOSql(ConexionSql conexion)
    {
        _conexion = conexion;
    }

    public List<Categoria> ListarTodos()
    {
        const string sql = "SELECT Id, Nombre FROM Categorias ORDER BY Nombre";

        var categorias = new List<Categoria>();

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
            categorias.Add(Mapear(lector));

        return categorias;
    }

    public Categoria? BuscarPorId(int id)
    {
        const string sql = "SELECT Id, Nombre FROM Categorias WHERE Id = @Id";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);
        using var lector = comando.ExecuteReader();

        return lector.Read() ? Mapear(lector) : null;
    }

    public Categoria Guardar(Categoria categoria)
    {
        const string sql = @"INSERT INTO Categorias (Nombre)
                              OUTPUT INSERTED.Id
                              VALUES (@Nombre)";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);

        categoria.Id = Convert.ToInt32(comando.ExecuteScalar());
        return categoria;
    }

    private static Categoria Mapear(SqlDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        Nombre = lector.GetString(1)
    };
}
