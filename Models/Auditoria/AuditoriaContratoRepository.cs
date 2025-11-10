using MySql.Data.MySqlClient;

namespace _Net.Models
{
    public class AuditoriasContratosRepository
    {
        private readonly string connectionString;

        public AuditoriasContratosRepository(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }
public IList<AuditoriaContrato> ObtenerTodos()
{
    var lista = new List<AuditoriaContrato>();

    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = @"SELECT a.IdRegistro, a.IdContrato, a.IdUsuarioCreador, a.FechaCreacion,
                              a.IdUsuarioFinalizador, a.FechaFinalizacion,
                              u1.Nombre AS NombreUsuarioCreador,
                              u2.Nombre AS NombreUsuarioFinalizador
                       FROM AuditoriaContratos a
                       JOIN Usuarios u1 ON a.IdUsuarioCreador = u1.IdUsuario
                       LEFT JOIN Usuarios u2 ON a.IdUsuarioFinalizador = u2.IdUsuario;";

        using (var command = new MySqlCommand(sql, connection))
        {
            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new AuditoriaContrato
                {
                    IdRegistro = reader.GetInt32("IdRegistro"),
                    IdContrato = reader.GetInt32("IdContrato"),
                    IdUsuarioCreador = reader.GetInt32("IdUsuarioCreador"),
                    FechaCreacion = reader.GetDateTime("FechaCreacion"),
                    IdUsuarioFinalizador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioFinalizador"))
                        ? null : reader.GetInt32("IdUsuarioFinalizador"),
                    FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal("FechaFinalizacion"))
                        ? null : reader.GetDateTime("FechaFinalizacion"),
                    NombreUsuarioCreador = reader.GetString("NombreUsuarioCreador"),
                    NombreUsuarioFinalizador = reader.IsDBNull(reader.GetOrdinal("NombreUsuarioFinalizador"))
                        ? null : reader.GetString("NombreUsuarioFinalizador")
                });
            }
            connection.Close();
        }
    }

    return lista;
}

        public int Insertar(AuditoriaContrato auditoria)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO AuditoriaContratos 
                               (IdContrato, IdUsuarioCreador, FechaCreacion)
                               VALUES (@IdContrato, @IdUsuarioCreador, @FechaCreacion);
                               SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", auditoria.IdContrato);
                    command.Parameters.AddWithValue("@IdUsuarioCreador", auditoria.IdUsuarioCreador);
                    command.Parameters.AddWithValue("@FechaCreacion", auditoria.FechaCreacion);

                    connection.Open();
                    var id = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                    return id;
                }
            }
        }

        public int FinalizarContrato(int idContrato, int idUsuarioFinalizador, DateTime fecha)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE AuditoriaContratos 
                               SET IdUsuarioFinalizador = @IdUsuarioFinalizador, 
                                   FechaFinalizacion = @FechaFinalizacion
                               WHERE IdContrato = @IdContrato";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuarioFinalizador", idUsuarioFinalizador);
                    command.Parameters.AddWithValue("@FechaFinalizacion", fecha);
                    command.Parameters.AddWithValue("@IdContrato", idContrato);

                    connection.Open();
                    var filas = command.ExecuteNonQuery();
                    connection.Close();
                    return filas;
                }
            }
        }
    }
}
