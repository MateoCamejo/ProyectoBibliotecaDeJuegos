using tpintegrador_psr2026.Api.DAO.Infraestructura;
using tpintegrador_psr2026.Api.Domain;
using tpintegrador_psr2026.Api.Domain.Enums;
using Microsoft.Data.SqlClient;

namespace tpintegrador_psr2026.Api.DAO.SqlServer;

public class JuegoDAOSql : IJuegoDAO
{
    private const string ColumnasJuego =
        "Id, Nombre, Descripcion, Precio, FechaLanzamiento, DesarrolladoraId, Estado";

    private readonly ConexionSql _conexion;

    public JuegoDAOSql(ConexionSql conexion)
    {
        _conexion = conexion;
    }

    public List<Juego> ListarTodos()
    {
        var sql = $"SELECT {ColumnasJuego} FROM Juegos";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();

        var juegos = EjecutarConsultaDeJuegos(conexion, sql);
        CompletarCategorias(conexion, juegos);
        return juegos;
    }

    public List<Juego> ListarPorNombre(string nombre)
    {
        var sql = $"SELECT {ColumnasJuego} FROM Juegos WHERE Nombre LIKE @Nombre";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Nombre", $"%{nombre}%");

        var juegos = EjecutarConsultaDeJuegos(comando);
        CompletarCategorias(conexion, juegos);
        return juegos;
    }

    public Juego? BuscarPorId(int id)
    {
        var sql = $"SELECT {ColumnasJuego} FROM Juegos WHERE Id = @Id";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", id);

        var juegos = EjecutarConsultaDeJuegos(comando);
        var juego = juegos.SingleOrDefault();

        if (juego is not null)
            juego.Categorias = ListarCategoriasDeJuego(conexion, juego.Id);

        return juego;
    }

    public List<Juego> ListarPorCategoria(int categoriaId)
    {
        var sql = $@"SELECT J.Id, J.Nombre, J.Descripcion, J.Precio, J.FechaLanzamiento, J.DesarrolladoraId, J.Estado
                     FROM Juegos J
                     INNER JOIN JuegoCategorias JC ON JC.JuegoId = J.Id
                     WHERE JC.CategoriaId = @CategoriaId";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@CategoriaId", categoriaId);

        var juegos = EjecutarConsultaDeJuegos(comando);
        CompletarCategorias(conexion, juegos);
        return juegos;
    }

    public List<Juego> ListarPorDesarrolladora(int desarrolladoraId)
    {
        var sql = $"SELECT {ColumnasJuego} FROM Juegos WHERE DesarrolladoraId = @DesarrolladoraId";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@DesarrolladoraId", desarrolladoraId);

        var juegos = EjecutarConsultaDeJuegos(comando);
        CompletarCategorias(conexion, juegos);
        return juegos;
    }

    public Juego Guardar(Juego juego)
    {
        const string sqlInsert = @"INSERT INTO Juegos
                                        (Nombre, Descripcion, Precio, FechaLanzamiento, DesarrolladoraId, Estado)
                                    OUTPUT INSERTED.Id
                                    VALUES
                                        (@Nombre, @Descripcion, @Precio, @FechaLanzamiento, @DesarrolladoraId, @Estado)";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            using (var comando = new SqlCommand(sqlInsert, conexion, transaccion))
            {
                AgregarParametrosJuego(comando, juego);
                juego.Id = Convert.ToInt32(comando.ExecuteScalar());
            }

            InsertarCategorias(conexion, transaccion, juego);

            transaccion.Commit();
            return juego;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public bool Actualizar(Juego juego)
    {
        const string sqlUpdate = @"UPDATE Juegos SET
                                        Nombre = @Nombre,
                                        Descripcion = @Descripcion,
                                        Precio = @Precio,
                                        FechaLanzamiento = @FechaLanzamiento,
                                        DesarrolladoraId = @DesarrolladoraId,
                                        Estado = @Estado
                                    WHERE Id = @Id";

        const string sqlBorrarCategorias = "DELETE FROM JuegoCategorias WHERE JuegoId = @JuegoId";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            int filasAfectadas;
            using (var comando = new SqlCommand(sqlUpdate, conexion, transaccion))
            {
                AgregarParametrosJuego(comando, juego);
                comando.Parameters.AddWithValue("@Id", juego.Id);
                filasAfectadas = comando.ExecuteNonQuery();
            }

            if (filasAfectadas == 0)
            {
                transaccion.Rollback();
                return false;
            }

            using (var comando = new SqlCommand(sqlBorrarCategorias, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@JuegoId", juego.Id);
                comando.ExecuteNonQuery();
            }

            InsertarCategorias(conexion, transaccion, juego);

            transaccion.Commit();
            return true;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    // ---------- Helpers privados ----------

    private static List<Juego> EjecutarConsultaDeJuegos(SqlConnection conexion, string sql)
    {
        using var comando = new SqlCommand(sql, conexion);
        return EjecutarConsultaDeJuegos(comando);
    }

    private static List<Juego> EjecutarConsultaDeJuegos(SqlCommand comando)
    {
        var juegos = new List<Juego>();

        using var lector = comando.ExecuteReader();
        while (lector.Read())
            juegos.Add(MapearJuego(lector));

        return juegos;
    }

    private static void CompletarCategorias(SqlConnection conexion, List<Juego> juegos)
    {
        foreach (var juego in juegos)
            juego.Categorias = ListarCategoriasDeJuego(conexion, juego.Id);
    }

    private static List<Categoria> ListarCategoriasDeJuego(SqlConnection conexion, int juegoId)
    {
        const string sql = @"SELECT C.Id, C.Nombre
                              FROM Categorias C
                              INNER JOIN JuegoCategorias JC ON JC.CategoriaId = C.Id
                              WHERE JC.JuegoId = @JuegoId";

        var categorias = new List<Categoria>();

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@JuegoId", juegoId);

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            categorias.Add(new Categoria
            {
                Id = lector.GetInt32(0),
                Nombre = lector.GetString(1)
            });
        }

        return categorias;
    }

    private static void InsertarCategorias(SqlConnection conexion, SqlTransaction transaccion, Juego juego)
    {
        const string sql = "INSERT INTO JuegoCategorias (JuegoId, CategoriaId) VALUES (@JuegoId, @CategoriaId)";

        foreach (var categoria in juego.Categorias)
        {
            using var comando = new SqlCommand(sql, conexion, transaccion);
            comando.Parameters.AddWithValue("@JuegoId", juego.Id);
            comando.Parameters.AddWithValue("@CategoriaId", categoria.Id);
            comando.ExecuteNonQuery();
        }
    }

    private static void AgregarParametrosJuego(SqlCommand comando, Juego juego)
    {
        comando.Parameters.AddWithValue("@Nombre", juego.Nombre);
        comando.Parameters.AddWithValue("@Descripcion", juego.Descripcion);
        comando.Parameters.AddWithValue("@Precio", juego.Precio);
        comando.Parameters.AddWithValue("@FechaLanzamiento", juego.FechaLanzamiento);
        comando.Parameters.AddWithValue("@DesarrolladoraId", juego.DesarrolladoraId);
        comando.Parameters.AddWithValue("@Estado", juego.Estado.ToString());
    }

    private static Juego MapearJuego(SqlDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        Nombre = lector.GetString(1),
        Descripcion = lector.IsDBNull(2) ? string.Empty : lector.GetString(2),
        Precio = lector.GetDecimal(3),
        FechaLanzamiento = lector.GetDateTime(4),
        DesarrolladoraId = lector.GetInt32(5),
        Estado = Enum.Parse<EstadoJuego>(lector.GetString(6))
    };
}
