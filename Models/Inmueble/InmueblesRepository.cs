using System.Data;
using MySql.Data.MySqlClient;

namespace _Net.Models;

public class InmueblesRepository : RepositoryBase, IRepositoryInmuebles
{
    public InmueblesRepository(IConfiguration configuration) : base(configuration) { }

    public Inmueble? ObtenerPorId(int id)
    {
        Inmueble? inmueble = null;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
                SELECT {nameof(Inmueble.IdInmueble)},
                       {nameof(Inmueble.Direccion)},
                       {nameof(Inmueble.Tipo)},
                       {nameof(Inmueble.Uso)},
                       {nameof(Inmueble.Ambientes)},
                       {nameof(Inmueble.Latitud)},
                       {nameof(Inmueble.Longitud)},
                       {nameof(Inmueble.IdPropietario)},
                       {nameof(Inmueble.Disponible)},
                       {nameof(Inmueble.FotoUrl)}
                FROM Inmuebles
                WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader[nameof(Inmueble.Direccion)] as string,
                            Tipo = reader[nameof(Inmueble.Tipo)] as string,
                            Uso = reader[nameof(Inmueble.Uso)] as string,
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                            FotoUrl = reader[nameof(Inmueble.FotoUrl)] as string
                        };
                    }
                }
                connection.Close();
            }
        }
        return inmueble;
    }

    public IList<Inmueble> ObtenerTodos()
    {
        List<Inmueble> inmuebles = new();
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
            SELECT i.{nameof(Inmueble.IdInmueble)},
                   i.{nameof(Inmueble.Direccion)},
                   i.{nameof(Inmueble.Tipo)},
                   i.{nameof(Inmueble.Uso)},
                   i.{nameof(Inmueble.Ambientes)},
                   i.{nameof(Inmueble.Latitud)},
                   i.{nameof(Inmueble.Longitud)},
                   i.{nameof(Inmueble.IdPropietario)},
                   i.{nameof(Inmueble.Disponible)},
                   i.{nameof(Inmueble.FotoUrl)},
                   p.Nombre AS PropietarioNombre,
                   p.Apellido AS PropietarioApellido
            FROM Inmuebles i
            INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario;";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader[nameof(Inmueble.Direccion)] as string,
                            Tipo = reader[nameof(Inmueble.Tipo)] as string,
                            Uso = reader[nameof(Inmueble.Uso)] as string,
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                            FotoUrl = reader[nameof(Inmueble.FotoUrl)] as string,
                            PropietarioNombre = reader["PropietarioNombre"] as string,
                            PropietarioApellido = reader["PropietarioApellido"] as string
                        });
                    }
                }
                connection.Close();
            }
        }
        return inmuebles;
    }

    public int Alta(Inmueble inmueble)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
                INSERT INTO Inmuebles 
                    ({nameof(Inmueble.Direccion)}, {nameof(Inmueble.Tipo)}, {nameof(Inmueble.Uso)}, 
                     {nameof(Inmueble.Ambientes)}, {nameof(Inmueble.Latitud)}, {nameof(Inmueble.Longitud)}, 
                     {nameof(Inmueble.IdPropietario)}, {nameof(Inmueble.Disponible)}, {nameof(Inmueble.FotoUrl)})
                VALUES 
                    (@{nameof(Inmueble.Direccion)}, @{nameof(Inmueble.Tipo)}, @{nameof(Inmueble.Uso)},
                     @{nameof(Inmueble.Ambientes)}, @{nameof(Inmueble.Latitud)}, @{nameof(Inmueble.Longitud)},
                     @{nameof(Inmueble.IdPropietario)}, @{nameof(Inmueble.Disponible)}, @{nameof(Inmueble.FotoUrl)});
                SELECT LAST_INSERT_ID();";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Direccion)}", inmueble.Direccion ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Tipo)}", inmueble.Tipo ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Uso)}", inmueble.Uso ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Ambientes)}", inmueble.Ambientes);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Latitud)}", inmueble.Latitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Longitud)}", inmueble.Longitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Disponible)}", inmueble.Disponible);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.FotoUrl)}", (object?)inmueble.FotoUrl ?? DBNull.Value);

                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                inmueble.IdInmueble = res;
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Inmueble inmueble)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
                UPDATE Inmuebles
                SET {nameof(Inmueble.Direccion)}   = @{nameof(Inmueble.Direccion)},
                    {nameof(Inmueble.Tipo)}        = @{nameof(Inmueble.Tipo)},
                    {nameof(Inmueble.Uso)}         = @{nameof(Inmueble.Uso)},
                    {nameof(Inmueble.Ambientes)}   = @{nameof(Inmueble.Ambientes)},
                    {nameof(Inmueble.Latitud)}     = @{nameof(Inmueble.Latitud)},
                    {nameof(Inmueble.Longitud)}    = @{nameof(Inmueble.Longitud)},
                    {nameof(Inmueble.IdPropietario)} = @{nameof(Inmueble.IdPropietario)},
                    {nameof(Inmueble.Disponible)}  = @{nameof(Inmueble.Disponible)},
                    {nameof(Inmueble.FotoUrl)}     = @{nameof(Inmueble.FotoUrl)}
                WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Direccion)}", inmueble.Direccion ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Tipo)}", inmueble.Tipo ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Uso)}", inmueble.Uso ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Ambientes)}", inmueble.Ambientes);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Latitud)}", inmueble.Latitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Longitud)}", inmueble.Longitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Disponible)}", inmueble.Disponible);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.FotoUrl)}", (object?)inmueble.FotoUrl ?? DBNull.Value);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", inmueble.IdInmueble);

                connection.Open();
                res = command.ExecuteNonQuery();
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
            string sql = $@"
                UPDATE Inmuebles
                SET {nameof(Inmueble.Disponible)} = false
                WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public IList<Inmueble> ObtenerTodosOPorFiltro(int? idPropietario = null, bool? disponible = null)
    {
        var lista = new List<Inmueble>();
        using (var connection = new MySqlConnection(ConectionString))
        {
            var sql = @"
            SELECT i.IdInmueble, i.Direccion, i.Tipo, i.Uso, i.Ambientes, 
                   i.Latitud, i.Longitud, i.IdPropietario, i.Disponible, i.FotoUrl,
                   p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido
            FROM Inmuebles i
            INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
            WHERE 1=1";

            if (idPropietario.HasValue)
                sql += " AND i.IdPropietario = @idPropietario";
            if (disponible.HasValue)
                sql += " AND i.Disponible = @disponible";

            using (var command = new MySqlCommand(sql, connection))
            {
                if (idPropietario.HasValue)
                    command.Parameters.AddWithValue("@idPropietario", idPropietario.Value);
                if (disponible.HasValue)
                    command.Parameters.AddWithValue("@disponible", disponible.Value);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader["Direccion"] as string,
                            Tipo = reader["Tipo"] as string,
                            Uso = reader["Uso"] as string,
                            Ambientes = reader.GetInt32("Ambientes"),
                            Latitud = reader.GetDecimal("Latitud"),
                            Longitud = reader.GetDecimal("Longitud"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Disponible = reader.GetBoolean("Disponible"),
                            FotoUrl = reader["FotoUrl"] as string,
                            PropietarioNombre = reader["PropietarioNombre"] as string,
                            PropietarioApellido = reader["PropietarioApellido"] as string
                        };
                        lista.Add(inmueble);
                    }
                }
                connection.Close();
            }
        }
        return lista;
    }

    public IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        var disponibles = new List<Inmueble>();
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = @"
                SELECT i.*
                FROM Inmuebles i
                WHERE i.IdInmueble NOT IN (
                    SELECT c.IdInmueble
                    FROM Contratos c
                    WHERE c.Vigente = true
                      AND (@FechaInicio <= c.FechaFin AND @FechaFin >= c.FechaInicio)
                );";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                command.Parameters.AddWithValue("@FechaFin", fechaFin);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        disponibles.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader["Direccion"] as string
                            // podés mapear más campos si necesitás
                        });
                    }
                }
                connection.Close();
            }
        }
        return disponibles;
    }
}



