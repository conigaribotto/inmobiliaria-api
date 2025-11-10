using System.Data;
using MySql.Data.MySqlClient;

namespace _Net.Models;

public class InquilinosRepository : RepositoryBase, IRepositoryInquilinos
{
    public InquilinosRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public Inquilino? ObtenerPorId(int id)
    {
        Inquilino? Inquilino = null;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"SELECT {nameof(Inquilino.IdInquilino)}, 
                               {nameof(Inquilino.Documento)}, 
                               {nameof(Inquilino.Nombre)}, 
                               {nameof(Inquilino.Apellido)}, 
                               {nameof(Inquilino.Telefono)}, 
                               {nameof(Inquilino.Mail)}
                        FROM Inquilinos
                        WHERE {nameof(Inquilino.IdInquilino)} = @{nameof(Inquilino.IdInquilino)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inquilino.IdInquilino)}", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Inquilino = new Inquilino
                        {
                            IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                            Documento = reader.GetInt32(nameof(Inquilino.Documento)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.Telefono)))
                                        ? null : reader.GetString(nameof(Inquilino.Telefono)),
                            Mail = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.Mail)))
                                        ? null : reader.GetString(nameof(Inquilino.Mail)),
                        };
                    }
                }
                connection.Close();
            }
        }
        return Inquilino;
    }

    public IList<Inquilino> ObtenerTodos()
    {
        List<Inquilino> Inquilinos = new List<Inquilino>();

        using (MySqlConnection connection = new MySqlConnection(ConectionString))
        {
            var query = $@"SELECT {nameof(Inquilino.IdInquilino)},
                                  {nameof(Inquilino.Documento)},
                                  {nameof(Inquilino.Nombre)},
                                  {nameof(Inquilino.Apellido)},
                                  {nameof(Inquilino.Telefono)},
                                  {nameof(Inquilino.Mail)}
                           FROM Inquilinos;";

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
                    Inquilinos.Add(new Inquilino
                    {
                        IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                        Documento = reader.GetInt32(nameof(Inquilino.Documento)),
                        Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                        Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                        Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                        Mail = reader.GetString(nameof(Inquilino.Mail))
                    });
                }
                connection.Close();
            }
        }
        return Inquilinos;
    }
    public int Alta(Inquilino Inquilino)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"INSERT INTO Inquilinos 
            ({nameof(Inquilino.Documento)}, 
            {nameof(Inquilino.Nombre)}, 
                {nameof(Inquilino.Apellido)}, 
                {nameof(Inquilino.Telefono)}, 
                {nameof(Inquilino.Mail)})
                VALUES (@{nameof(Inquilino.Documento)}, 
                @{nameof(Inquilino.Nombre)}, 
                        @{nameof(Inquilino.Apellido)}, 
                        @{nameof(Inquilino.Telefono)}, 
                        @{nameof(Inquilino.Mail)});
                SELECT LAST_INSERT_ID();"; //Devuelve el ID

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Documento)}", Inquilino.Documento);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Nombre)}", Inquilino.Nombre ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Apellido)}", Inquilino.Apellido ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Telefono)}", Inquilino.Telefono ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Mail)}", Inquilino.Mail ?? "");

                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                Inquilino.IdInquilino = res;
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
            string sql = $"DELETE FROM Inquilinos WHERE {nameof(Inquilino.IdInquilino)} = @{nameof(Inquilino.IdInquilino)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inquilino.IdInquilino)}", id);

                connection.Open();
                res = command.ExecuteNonQuery(); // devuelve cuántas filas fueron eliminadas
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Inquilino Inquilino)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
            UPDATE Inquilinos
            SET {nameof(Inquilino.Documento)} = @{nameof(Inquilino.Documento)},
                {nameof(Inquilino.Nombre)} = @{nameof(Inquilino.Nombre)},
                {nameof(Inquilino.Apellido)} = @{nameof(Inquilino.Apellido)},
                {nameof(Inquilino.Telefono)} = @{nameof(Inquilino.Telefono)},
                {nameof(Inquilino.Mail)} = @{nameof(Inquilino.Mail)}
            WHERE {nameof(Inquilino.IdInquilino)} = @{nameof(Inquilino.IdInquilino)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Documento)}", Inquilino.Documento);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Nombre)}", Inquilino.Nombre ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Apellido)}", Inquilino.Apellido ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Telefono)}", Inquilino.Telefono ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Mail)}", Inquilino.Mail ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inquilino.IdInquilino)}", Inquilino.IdInquilino);

                connection.Open();
                res = command.ExecuteNonQuery(); // Devuelve la cantidad de filas afectadas
                connection.Close();
            }
        }
        return res;
    }



}
