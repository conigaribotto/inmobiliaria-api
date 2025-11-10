using System.Data;
using MySql.Data.MySqlClient;

namespace _Net.Models;

public class PropietariosRepository : RepositoryBase, IRepositoryPropietario
{
    public PropietariosRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public Propietario? ObtenerPorId(int id)
    {
        Propietario? propietario = null;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"SELECT {nameof(Propietario.IdPropietario)}, 
                               {nameof(Propietario.Documento)}, 
                               {nameof(Propietario.Nombre)}, 
                               {nameof(Propietario.Apellido)}, 
                               {nameof(Propietario.Telefono)}, 
                               {nameof(Propietario.Mail)}
                        FROM Propietarios
                        WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Propietario.IdPropietario)}", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        propietario = new Propietario
                        {
                            IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                            Documento = reader.GetInt32(nameof(Propietario.Documento)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Telefono)))
                                        ? null : reader.GetString(nameof(Propietario.Telefono)),
                            Mail = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Mail)))
                                        ? null : reader.GetString(nameof(Propietario.Mail)),
                        };
                    }
                }
                connection.Close();
            }
        }
        return propietario;
    }

    public IList<Propietario> ObtenerTodos()
    {
        List<Propietario> propietarios = new List<Propietario>();

        using (MySqlConnection connection = new MySqlConnection(ConectionString))
        {
            var query = $@"SELECT {nameof(Propietario.IdPropietario)},
                                  {nameof(Propietario.Documento)},
                                  {nameof(Propietario.Nombre)},
                                  {nameof(Propietario.Apellido)},
                                  {nameof(Propietario.Telefono)},
                                  {nameof(Propietario.Mail)}
                           FROM propietarios;";

            //Ussing para asegurar el cierre y liberación de recursos (dispose)
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                //Establece que es SQLpuro y no SP
                command.CommandType = CommandType.Text;
                connection.Open();
                //Señala el resultado como una matriz y lee a medida que avanza
                var reader = command.ExecuteReader();
                //Lee mientras haya registros
                while (reader.Read())
                {
                    propietarios.Add(new Propietario
                    {
                        IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                        Documento = reader.GetInt32(nameof(Propietario.Documento)),
                        Nombre = reader.GetString(nameof(Propietario.Nombre)),
                        Apellido = reader.GetString(nameof(Propietario.Apellido)),
                        Telefono = reader.GetString(nameof(Propietario.Telefono)),
                        Mail = reader.GetString(nameof(Propietario.Mail))
                    });
                }
                connection.Close();
            }
        }
        return propietarios;
    }

    // Models/PropietariosRepository.cs  (añadir este método)
public Propietario? ObtenerPorEmail(string mail)
{
    Propietario? p = null;
    using var connection = new MySql.Data.MySqlClient.MySqlConnection(ConectionString);
    var sql = @"SELECT IdPropietario, Nombre, Apellido, Telefono, Mail
                FROM Propietarios
                WHERE Mail = @mail LIMIT 1;";
    using var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, connection);
    cmd.Parameters.AddWithValue("@mail", mail);
    connection.Open();
    using var r = cmd.ExecuteReader();
    if (r.Read())
    {
        p = new Propietario
        {
            IdPropietario = r.GetInt32("IdPropietario"),
            Nombre = r.GetString("Nombre"),
            Apellido = r.GetString("Apellido"),
            Telefono = r.IsDBNull(r.GetOrdinal("Telefono")) ? null : r.GetString("Telefono"),
            Mail = r.GetString("Mail")
        };
    }
    return p;
}

    public int Alta(Propietario propietario)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"INSERT INTO propietarios 
            ({nameof(Propietario.Documento)}, 
            {nameof(Propietario.Nombre)}, 
                {nameof(Propietario.Apellido)}, 
                {nameof(Propietario.Telefono)}, 
                {nameof(Propietario.Mail)})
                VALUES (@{nameof(Propietario.Documento)}, 
                @{nameof(Propietario.Nombre)}, 
                        @{nameof(Propietario.Apellido)}, 
                        @{nameof(Propietario.Telefono)}, 
                        @{nameof(Propietario.Mail)});
                SELECT LAST_INSERT_ID();"; //Devuelve el ID

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Propietario.Documento)}", propietario.Documento);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Nombre)}", propietario.Nombre ?? "");
                command.Parameters.AddWithValue($"@{nameof(Propietario.Apellido)}", propietario.Apellido ?? "");
                command.Parameters.AddWithValue($"@{nameof(Propietario.Telefono)}", propietario.Telefono ?? "");
                command.Parameters.AddWithValue($"@{nameof(Propietario.Mail)}", propietario.Mail ?? "");

                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                propietario.IdPropietario = res;
                connection.Close();
            }
        }
        return res;
    }

    public int Baja(int id)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $"DELETE FROM Propietarios WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Propietario.IdPropietario)}", id);

                connection.Open();
                res = command.ExecuteNonQuery(); // devuelve cuántas filas fueron eliminadas
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Propietario propietario)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
            UPDATE propietarios
            SET {nameof(Propietario.Documento)} = @{nameof(Propietario.Documento)},
                {nameof(Propietario.Nombre)} = @{nameof(Propietario.Nombre)},
                {nameof(Propietario.Apellido)} = @{nameof(Propietario.Apellido)},
                {nameof(Propietario.Telefono)} = @{nameof(Propietario.Telefono)},
                {nameof(Propietario.Mail)} = @{nameof(Propietario.Mail)}
            WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Propietario.Documento)}", propietario.Documento);
                command.Parameters.AddWithValue($"@{nameof(Propietario.Nombre)}", propietario.Nombre ?? "");
                command.Parameters.AddWithValue($"@{nameof(Propietario.Apellido)}", propietario.Apellido ?? "");
                command.Parameters.AddWithValue($"@{nameof(Propietario.Telefono)}", propietario.Telefono ?? "");
                command.Parameters.AddWithValue($"@{nameof(Propietario.Mail)}", propietario.Mail ?? "");
                command.Parameters.AddWithValue($"@{nameof(Propietario.IdPropietario)}", propietario.IdPropietario);

                connection.Open();
                res = command.ExecuteNonQuery(); // Devuelve la cantidad de filas afectadas
                connection.Close();
            }
        }
        return res;
    }
}





