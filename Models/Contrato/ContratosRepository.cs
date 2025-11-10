using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace _Net.Models;

public class ContratosRepository : RepositoryBase, IRepositoryContratos
{
    public ContratosRepository(IConfiguration configuration) : base(configuration) { }

    public Contrato? ObtenerPorId(int id)
    {
        Contrato? contrato = null;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
                SELECT {nameof(Contrato.IdContrato)},
                       {nameof(Contrato.IdInquilino)},
                       {nameof(Contrato.IdInmueble)},
                       {nameof(Contrato.FechaInicio)},
                       {nameof(Contrato.FechaFin)},
                       {nameof(Contrato.ValorMensual)},
                       {nameof(Contrato.Vigente)}
                FROM Contratos
                WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        contrato = new Contrato
                        {
                            IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                            IdInquilino = reader.GetInt32(nameof(Contrato.IdInquilino)),
                            IdInmueble = reader.GetInt32(nameof(Contrato.IdInmueble)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                            ValorMensual = reader.GetDouble(nameof(Contrato.ValorMensual)),
                            Vigente = reader.GetBoolean(nameof(Contrato.Vigente))
                        };
                    }
                }
                connection.Close();
            }
        }
        return contrato;
    }

    public IList<Contrato> ObtenerTodos()
    {
        IList<Contrato> contratos = new List<Contrato>();

        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = @"
                SELECT c.IdContrato, c.IdInquilino, c.IdInmueble, c.FechaInicio, c.FechaFin, 
                       c.ValorMensual, c.Vigente,
                       i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                       inm.Direccion AS InmuebleDireccion
                FROM Contratos c
                INNER JOIN Inquilinos i ON c.IdInquilino = i.IdInquilino
                INNER JOIN Inmuebles inm ON c.IdInmueble = inm.IdInmueble;";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contratos.Add(new Contrato
                        {
                            IdContrato = reader.GetInt32("IdContrato"),
                            IdInquilino = reader.GetInt32("IdInquilino"),
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            FechaInicio = reader.GetDateTime("FechaInicio"),
                            FechaFin = reader.GetDateTime("FechaFin"),
                            ValorMensual = reader.GetDouble("ValorMensual"),
                            Vigente = reader.GetBoolean("Vigente"),
                            InquilinoNombre = reader.GetString("InquilinoNombre"),
                            InquilinoApellido = reader.GetString("InquilinoApellido"),
                            InmuebleDireccion = reader.GetString("InmuebleDireccion")
                        });
                    }
                }
                connection.Close();
            }
        }

        return contratos;
    }

    public IList<Contrato> ObtenerTodosOPorFiltros(int? dias = null, bool? vigente = null, int? idInquilino = null)
    {
        List<Contrato> contratos = new();
        using (var connection = new MySqlConnection(ConectionString))
        {
            var sql = new StringBuilder(@"
                SELECT c.IdContrato, c.IdInquilino, c.IdInmueble, c.FechaInicio, c.FechaFin, 
                       c.ValorMensual, c.Vigente,
                       i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                       inm.Direccion AS InmuebleDireccion
                FROM Contratos c
                INNER JOIN Inquilinos i ON c.IdInquilino = i.IdInquilino
                INNER JOIN Inmuebles inm ON c.IdInmueble = inm.IdInmueble
                WHERE 1=1");

            if (dias.HasValue)
                sql.Append(" AND DATEDIFF(c.FechaFin, CURDATE()) <= @dias");
            if (vigente.HasValue)
                sql.Append(" AND c.Vigente = @vigente");
            if (idInquilino.HasValue)
                sql.Append(" AND c.IdInquilino = @idInquilino");

            using (var command = new MySqlCommand(sql.ToString(), connection))
            {
                if (dias.HasValue)
                    command.Parameters.AddWithValue("@dias", dias.Value);
                if (vigente.HasValue)
                    command.Parameters.AddWithValue("@vigente", vigente.Value);
                if (idInquilino.HasValue)
                    command.Parameters.AddWithValue("@idInquilino", idInquilino.Value);

                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    contratos.Add(new Contrato
                    {
                        IdContrato = reader.GetInt32("IdContrato"),
                        IdInquilino = reader.GetInt32("IdInquilino"),
                        IdInmueble = reader.GetInt32("IdInmueble"),
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        ValorMensual = reader.GetDouble("ValorMensual"),
                        Vigente = reader.GetBoolean("Vigente"),
                        InquilinoNombre = reader.GetString("InquilinoNombre"),
                        InquilinoApellido = reader.GetString("InquilinoApellido"),
                        InmuebleDireccion = reader.GetString("InmuebleDireccion")
                    });
                }
                connection.Close();
            }
        }
        return contratos;
    }

    public int Alta(Contrato contrato)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
                INSERT INTO Contratos 
                    ({nameof(Contrato.IdInquilino)}, 
                     {nameof(Contrato.IdInmueble)}, 
                     {nameof(Contrato.FechaInicio)}, 
                     {nameof(Contrato.FechaFin)}, 
                     {nameof(Contrato.ValorMensual)}, 
                     {nameof(Contrato.Vigente)})
                VALUES
                    (@{nameof(Contrato.IdInquilino)}, 
                     @{nameof(Contrato.IdInmueble)}, 
                     @{nameof(Contrato.FechaInicio)}, 
                     @{nameof(Contrato.FechaFin)}, 
                     @{nameof(Contrato.ValorMensual)}, 
                     @{nameof(Contrato.Vigente)});
                SELECT LAST_INSERT_ID();";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInquilino)}", contrato.IdInquilino);
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInmueble)}", contrato.IdInmueble);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaInicio)}", contrato.FechaInicio);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaFin)}", contrato.FechaFin);
                command.Parameters.AddWithValue($"@{nameof(Contrato.ValorMensual)}", contrato.ValorMensual);
                command.Parameters.AddWithValue($"@{nameof(Contrato.Vigente)}", contrato.Vigente);

                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                contrato.IdContrato = res;
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Contrato contrato)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
                UPDATE Contratos
                SET {nameof(Contrato.IdInquilino)}  = @{nameof(Contrato.IdInquilino)},
                    {nameof(Contrato.IdInmueble)}   = @{nameof(Contrato.IdInmueble)},
                    {nameof(Contrato.FechaInicio)}  = @{nameof(Contrato.FechaInicio)},
                    {nameof(Contrato.FechaFin)}     = @{nameof(Contrato.FechaFin)},
                    {nameof(Contrato.ValorMensual)} = @{nameof(Contrato.ValorMensual)},
                    {nameof(Contrato.Vigente)}      = @{nameof(Contrato.Vigente)}
                WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInquilino)}", contrato.IdInquilino);
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInmueble)}", contrato.IdInmueble);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaInicio)}", contrato.FechaInicio);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaFin)}", contrato.FechaFin);
                command.Parameters.AddWithValue($"@{nameof(Contrato.ValorMensual)}", contrato.ValorMensual);
                command.Parameters.AddWithValue($"@{nameof(Contrato.Vigente)}", contrato.Vigente);
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", contrato.IdContrato);

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
                UPDATE Contratos
                SET {nameof(Contrato.Vigente)} = false
                WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public bool ExisteSuperposicion(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idContratoExcluir = null)
    {
        bool existe = false;

        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = @"
                SELECT COUNT(*) 
                FROM Contratos 
                WHERE IdInmueble = @IdInmueble 
                  AND Vigente = true
                  AND (@FechaInicio <= FechaFin AND @FechaFin >= FechaInicio)";

            if (idContratoExcluir.HasValue)
                sql += " AND IdContrato <> @IdContrato";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdInmueble", idInmueble);
                command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                command.Parameters.AddWithValue("@FechaFin", fechaFin);

                if (idContratoExcluir.HasValue)
                    command.Parameters.AddWithValue("@IdContrato", idContratoExcluir.Value);

                connection.Open();
                existe = Convert.ToInt32(command.ExecuteScalar()) > 0;
                connection.Close();
            }
        }

        return existe;
    }

    // NUEVO: para listar contratos por inmueble (usado por la API)
    public IList<Contrato> ObtenerPorInmueble(int idInmueble)
    {
        IList<Contrato> contratos = new List<Contrato>();
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = @"
                SELECT c.IdContrato, c.IdInquilino, c.IdInmueble, c.FechaInicio, c.FechaFin,
                       c.ValorMensual, c.Vigente
                FROM Contratos c
                WHERE c.IdInmueble = @IdInmueble;";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdInmueble", idInmueble);
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    contratos.Add(new Contrato
                    {
                        IdContrato = reader.GetInt32("IdContrato"),
                        IdInquilino = reader.GetInt32("IdInquilino"),
                        IdInmueble = reader.GetInt32("IdInmueble"),
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        ValorMensual = reader.GetDouble("ValorMensual"),
                        Vigente = reader.GetBoolean("Vigente")
                    });
                }
                connection.Close();
            }
        }
        return contratos;
    }
}

