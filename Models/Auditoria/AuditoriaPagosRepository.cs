using MySql.Data.MySqlClient;

namespace _Net.Models;

public class AuditoriaPagosRepository : RepositoryBase
{
    public AuditoriaPagosRepository(IConfiguration configuration) : base(configuration) { }

    public void InsertarCreacion(int idPago, int idUsuario)
    {
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = @"INSERT INTO AuditoriaPagos 
                           (IdPago, IdUsuarioCreador, FechaCreacion) 
                           VALUES (@IdPago, @IdUsuario, @FechaCreacion);";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdPago", idPago);
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void RegistrarAnulacion(int idPago, int idUsuario)
    {
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = @"UPDATE AuditoriaPagos
                           SET IdUsuarioAnulador = @IdUsuario,
                               FechaAnulacion = @FechaAnulacion
                           WHERE IdPago = @IdPago;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdPago", idPago);
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@FechaAnulacion", DateTime.Now);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public IList<AuditoriaPago> ObtenerTodos()
{
    var lista = new List<AuditoriaPago>();
    using (var connection = new MySqlConnection(ConectionString))
    {
        string sql = @"SELECT a.IdRegistro, a.IdPago, a.IdUsuarioCreador, a.FechaCreacion,
                              a.IdUsuarioAnulador, a.FechaAnulacion,
                              u1.Nombre AS NombreUsuarioCreador,
                              u2.Nombre AS NombreUsuarioAnulador
                       FROM AuditoriaPagos a
                       JOIN Usuarios u1 ON a.IdUsuarioCreador = u1.IdUsuario
                       LEFT JOIN Usuarios u2 ON a.IdUsuarioAnulador = u2.IdUsuario;";

        using (var command = new MySqlCommand(sql, connection))
        {
            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new AuditoriaPago
                {
                    IdRegistro = reader.GetInt32("IdRegistro"),
                    IdPago = reader.GetInt32("IdPago"),
                    IdUsuarioCreador = reader.GetInt32("IdUsuarioCreador"),
                    FechaCreacion = reader.GetDateTime("FechaCreacion"),
                    IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioAnulador"))
                        ? null : reader.GetInt32("IdUsuarioAnulador"),
                    FechaAnulacion = reader.IsDBNull(reader.GetOrdinal("FechaAnulacion"))
                        ? null : reader.GetDateTime("FechaAnulacion"),
                    NombreUsuarioCreador = reader.GetString("NombreUsuarioCreador"),
                    NombreUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("NombreUsuarioAnulador"))
                        ? null : reader.GetString("NombreUsuarioAnulador")
                });
            }
        }
    }
    return lista;
}


}
