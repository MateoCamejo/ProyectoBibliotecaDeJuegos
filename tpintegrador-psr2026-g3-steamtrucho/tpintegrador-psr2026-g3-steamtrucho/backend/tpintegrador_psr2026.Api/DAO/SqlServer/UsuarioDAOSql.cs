using tpintegrador_psr2026.Api.DAO.Infraestructura;
using tpintegrador_psr2026.Api.Domain;
using Microsoft.Data.SqlClient;

namespace tpintegrador_psr2026.Api.DAO.SqlServer;

public class UsuarioDAOSql : IUsuarioDAO
{
    private readonly ConexionSql _conexion;

    public UsuarioDAOSql(ConexionSql conexion)
    {
        _conexion = conexion;
    }

    public Usuario? BuscarPorId(int id)
    {
        const string sql = "SELECT Id, Nombre, Email FROM Usuarios WHERE Id = @Id";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();

        Usuario? usuario;
        using (var comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@Id", id);
            using var lector = comando.ExecuteReader();
            usuario = lector.Read() ? MapearUsuario(lector) : null;
        }

        if (usuario is not null)
            usuario.Biblioteca = CargarBiblioteca(conexion, usuario.Id);

        return usuario;
    }

    public Usuario? BuscarPorEmail(string email)
    {
        const string sql = "SELECT Id, Nombre, Email FROM Usuarios WHERE Email = @Email";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Email", email);
        using var lector = comando.ExecuteReader();

        return lector.Read() ? MapearUsuario(lector) : null;
    }

    public Usuario Guardar(Usuario usuario)
    {
        const string sql = @"INSERT INTO Usuarios (Nombre, Email)
                              OUTPUT INSERTED.Id
                              VALUES (@Nombre, @Email)";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
        comando.Parameters.AddWithValue("@Email", usuario.Email);

        usuario.Id = Convert.ToInt32(comando.ExecuteScalar());
        usuario.Biblioteca = new Biblioteca { Id = usuario.Id, UsuarioId = usuario.Id };

        return usuario;
    }

    public void AgregarItemBiblioteca(int usuarioId, ItemBiblioteca item)
    {
        const string sql = @"INSERT INTO ItemsBiblioteca
                                  (UsuarioId, JuegoId, FechaAdquisicion, HorasJugadas, UltimaVezUsado)
                              OUTPUT INSERTED.Id
                              VALUES
                                  (@UsuarioId, @JuegoId, @FechaAdquisicion, @HorasJugadas, @UltimaVezUsado)";

        using var conexion = _conexion.CrearConexion();
        conexion.Open();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@UsuarioId", usuarioId);
        comando.Parameters.AddWithValue("@JuegoId", item.JuegoId);
        comando.Parameters.AddWithValue("@FechaAdquisicion", item.FechaAdquisicion);
        comando.Parameters.AddWithValue("@HorasJugadas", item.HorasJugadas);
        comando.Parameters.AddWithValue("@UltimaVezUsado", item.UltimaVezUsado ?? (object)DBNull.Value);

        item.Id = Convert.ToInt32(comando.ExecuteScalar());
    }

    private static Biblioteca CargarBiblioteca(SqlConnection conexion, int usuarioId)
    {
        const string sql = @"SELECT Id, JuegoId, FechaAdquisicion, HorasJugadas, UltimaVezUsado
                              FROM ItemsBiblioteca
                              WHERE UsuarioId = @UsuarioId";

        var biblioteca = new Biblioteca { Id = usuarioId, UsuarioId = usuarioId };

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@UsuarioId", usuarioId);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            biblioteca.Items.Add(new ItemBiblioteca
            {
                Id = lector.GetInt32(0),
                JuegoId = lector.GetInt32(1),
                FechaAdquisicion = lector.GetDateTime(2),
                HorasJugadas = lector.GetDouble(3),
                UltimaVezUsado = lector.IsDBNull(4) ? null : lector.GetDateTime(4)
            });
        }

        return biblioteca;
    }

    private static Usuario MapearUsuario(SqlDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        Nombre = lector.GetString(1),
        Email = lector.GetString(2)
    };
}
