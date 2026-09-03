using tpintegrador_psr2026.Api.DAO.Infraestructura;
using tpintegrador_psr2026.Api.Domain;
using Microsoft.Data.SqlClient;

namespace tpintegrador_psr2026.Api.DAO.SqlServer;

public class CampanaDAOSql : ICampanaDAO
{
    private readonly ConexionSql _conexion;

    public CampanaDAOSql(ConexionSql conexion)
    {
        _conexion = conexion;
    }

    public List<Campana> ListarActivas(DateTime fecha)
    {
        const string sql = @"SELECT Id, Nombre, FechaInicio, FechaFin, PorcentajeDescuento, CategoriaId, DesarrolladoraId
                              FROM Campanas
                              WHERE FechaInicio <= @Fecha AND FechaFin >= @Fecha";

        var campanas = new List<Campana>();

        using var conexion = _conexion.CrearConexion();
        conexion.Open();

        using (var comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@Fecha", fecha);
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                campanas.Add(MapearCampana(lector));
        }

        foreach (var campana in campanas)
            campana.JuegosAfectados = ListarJuegosAfectados(conexion, campana.Id);

        return campanas;
    }

    // Inserta la Campana y, si tiene una selección específica de
    // juegos, las filas de CampanaJuegos, en una única transacción.
    public Campana Guardar(Campana campana)
    {
        const string sqlCampana = @"INSERT INTO Campanas
                                         (Nombre, FechaInicio, FechaFin, PorcentajeDescuento, CategoriaId, DesarrolladoraId)
                                     OUTPUT INSERTED.Id
                                     VALUES
                                         (@Nombre, @FechaInicio, @FechaFin, @PorcentajeDescuento, @CategoriaId, @DesarrolladoraId)";

        const string sqlJuego = "INSERT INTO CampanaJuegos (CampanaId, JuegoId) VALUES (@CampanaId, @JuegoId)";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            using (var comando = new SqlCommand(sqlCampana, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Nombre", campana.Nombre);
                comando.Parameters.AddWithValue("@FechaInicio", campana.FechaInicio);
                comando.Parameters.AddWithValue("@FechaFin", campana.FechaFin);
                comando.Parameters.AddWithValue("@PorcentajeDescuento", campana.PorcentajeDescuento);
                comando.Parameters.AddWithValue("@CategoriaId", (object?)campana.CategoriaId ?? DBNull.Value);
                comando.Parameters.AddWithValue("@DesarrolladoraId", (object?)campana.DesarrolladoraId ?? DBNull.Value);
                campana.Id = Convert.ToInt32(comando.ExecuteScalar());
            }

            if (campana.JuegosAfectados is not null)
            {
                foreach (var juegoId in campana.JuegosAfectados)
                {
                    using var comando = new SqlCommand(sqlJuego, conexion, transaccion);
                    comando.Parameters.AddWithValue("@CampanaId", campana.Id);
                    comando.Parameters.AddWithValue("@JuegoId", juegoId);
                    comando.ExecuteNonQuery();
                }
            }

            transaccion.Commit();
            return campana;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static List<int> ListarJuegosAfectados(SqlConnection conexion, int campanaId)
    {
        const string sql = "SELECT JuegoId FROM CampanaJuegos WHERE CampanaId = @CampanaId";

        var juegoIds = new List<int>();

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@CampanaId", campanaId);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
            juegoIds.Add(lector.GetInt32(0));

        return juegoIds;
    }

    private static Campana MapearCampana(SqlDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        Nombre = lector.GetString(1),
        FechaInicio = lector.GetDateTime(2),
        FechaFin = lector.GetDateTime(3),
        PorcentajeDescuento = lector.GetDecimal(4),
        CategoriaId = lector.IsDBNull(5) ? null : lector.GetInt32(5),
        DesarrolladoraId = lector.IsDBNull(6) ? null : lector.GetInt32(6)
    };
}
