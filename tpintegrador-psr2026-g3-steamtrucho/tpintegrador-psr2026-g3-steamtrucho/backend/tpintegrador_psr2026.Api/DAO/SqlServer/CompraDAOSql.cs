using tpintegrador_psr2026.Api.DAO.Infraestructura;
using tpintegrador_psr2026.Api.Domain;
using tpintegrador_psr2026.Api.Domain.Enums;
using Microsoft.Data.SqlClient;

namespace tpintegrador_psr2026.Api.DAO.SqlServer;

public class CompraDAOSql : ICompraDAO
{
    private readonly ConexionSql _conexion;

    public CompraDAOSql(ConexionSql conexion)
    {
        _conexion = conexion;
    }

    public Compra? BuscarPorId(int id)
    {
        const string sql = "SELECT Id, UsuarioId, Fecha, ImporteFinal, Estado FROM Compras WHERE Id = @Id";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();

        Compra? compra;
        using (var comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@Id", id);
            using var lector = comando.ExecuteReader();
            compra = lector.Read() ? MapearCompra(lector) : null;
        }

        if (compra is not null)
            compra.Detalles = ListarDetalles(conexion, compra.Id);

        return compra;
    }

    public List<Compra> ListarPorUsuario(int usuarioId)
    {
        const string sql = @"SELECT Id, UsuarioId, Fecha, ImporteFinal, Estado
                              FROM Compras WHERE UsuarioId = @UsuarioId
                              ORDER BY Fecha DESC";

        var compras = new List<Compra>();

        using var conexion = _conexion.CrearConexion();
        conexion.Open();

        using (var comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@UsuarioId", usuarioId);
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                compras.Add(MapearCompra(lector));
        }

        foreach (var compra in compras)
            compra.Detalles = ListarDetalles(conexion, compra.Id);

        return compras;
    }

    // Inserta la Compra y todos sus DetalleCompra en una única
    // transacción: si falla un detalle, no queda una compra "a medias".
    public Compra Guardar(Compra compra)
    {
        const string sqlCompra = @"INSERT INTO Compras (UsuarioId, Fecha, ImporteFinal, Estado)
                                    OUTPUT INSERTED.Id
                                    VALUES (@UsuarioId, @Fecha, @ImporteFinal, @Estado)";

        const string sqlDetalle = @"INSERT INTO DetallesCompra (CompraId, JuegoId, PrecioFinal)
                                     OUTPUT INSERTED.Id
                                     VALUES (@CompraId, @JuegoId, @PrecioFinal)";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            using (var comando = new SqlCommand(sqlCompra, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@UsuarioId", compra.UsuarioId);
                comando.Parameters.AddWithValue("@Fecha", compra.Fecha);
                comando.Parameters.AddWithValue("@ImporteFinal", compra.ImporteFinal);
                comando.Parameters.AddWithValue("@Estado", compra.Estado.ToString());
                compra.Id = Convert.ToInt32(comando.ExecuteScalar());
            }

            foreach (var detalle in compra.Detalles)
            {
                using var comando = new SqlCommand(sqlDetalle, conexion, transaccion);
                comando.Parameters.AddWithValue("@CompraId", compra.Id);
                comando.Parameters.AddWithValue("@JuegoId", detalle.JuegoId);
                comando.Parameters.AddWithValue("@PrecioFinal", detalle.PrecioFinal);
                detalle.Id = Convert.ToInt32(comando.ExecuteScalar());
            }

            transaccion.Commit();
            return compra;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public bool ActualizarEstado(int id, EstadoCompra estado)
    {
        const string sql = "UPDATE Compras SET Estado = @Estado WHERE Id = @Id";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Estado", estado.ToString());
        comando.Parameters.AddWithValue("@Id", id);

        return comando.ExecuteNonQuery() > 0;
    }

    private static List<DetalleCompra> ListarDetalles(SqlConnection conexion, int compraId)
    {
        const string sql = "SELECT Id, JuegoId, PrecioFinal FROM DetallesCompra WHERE CompraId = @CompraId";

        var detalles = new List<DetalleCompra>();

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@CompraId", compraId);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            detalles.Add(new DetalleCompra
            {
                Id = lector.GetInt32(0),
                JuegoId = lector.GetInt32(1),
                PrecioFinal = lector.GetDecimal(2)
            });
        }

        return detalles;
    }

    private static Compra MapearCompra(SqlDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        UsuarioId = lector.GetInt32(1),
        Fecha = lector.GetDateTime(2),
        ImporteFinal = lector.GetDecimal(3),
        Estado = Enum.Parse<EstadoCompra>(lector.GetString(4))
    };
}
