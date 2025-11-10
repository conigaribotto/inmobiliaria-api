using System.Data;
using MySql.Data.MySqlClient;

namespace _Net.Models;

public class PagosRepository : RepositoryBase, IRepositoryPagos
{
    public PagosRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public Pago? ObtenerPorId(int id)
    {
        Pago? pago = null;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"SELECT {nameof(Pago.IdPago)},
                                   {nameof(Pago.IdContrato)},
                                   {nameof(Pago.concepto)},
                                   {nameof(Pago.importe)},
                                   {nameof(Pago.Fecha)},
                                   {nameof(Pago.anulado)}
                            FROM Pagos
                            WHERE {nameof(Pago.IdPago)} = @{nameof(Pago.IdPago)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Pago.IdPago)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pago = new Pago
                        {
                            IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                            IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                            concepto = reader.GetString(nameof(Pago.concepto)),
                            importe = reader.GetDouble(nameof(Pago.importe)),
                            Fecha = reader.GetDateTime(nameof(Pago.Fecha)),
                            anulado = reader.GetBoolean(nameof(Pago.anulado))
                        };
                    }
                }
                connection.Close();
            }
        }
        return pago;
    }

    public IList<Pago> ObtenerTodos()
    {
        List<Pago> pagos = new List<Pago>();

        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
                SELECT {nameof(Pago.IdPago)},
                    {nameof(Pago.IdContrato)},
                    {nameof(Pago.concepto)},
                    {nameof(Pago.importe)},
                    {nameof(Pago.Fecha)},
                    {nameof(Pago.anulado)}
                FROM Pagos";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pagos.Add(new Pago
                        {
                            IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                            IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                            concepto = reader.GetString(nameof(Pago.concepto)),
                            importe = reader.GetDouble(nameof(Pago.importe)),
                            Fecha = reader.GetDateTime(nameof(Pago.Fecha)),
                            anulado = reader.GetBoolean(nameof(Pago.anulado))
                        });
                    }
                }
                connection.Close();
            }
        }
        return pagos;
    }

    public IList<Pago> ObtenerPorContrato(int idContrato)
    {
        List<Pago> pagos = new List<Pago>();

        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"
                SELECT {nameof(Pago.IdPago)},
                    {nameof(Pago.IdContrato)},
                    {nameof(Pago.concepto)},
                    {nameof(Pago.importe)},
                    {nameof(Pago.Fecha)},
                    {nameof(Pago.anulado)}
                FROM Pagos
                WHERE IdContrato = @IdContrato";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdContrato", idContrato);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pagos.Add(new Pago
                        {
                            IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                            IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                            concepto = reader.GetString(nameof(Pago.concepto)),
                            importe = reader.GetDouble(nameof(Pago.importe)),
                            Fecha = reader.GetDateTime(nameof(Pago.Fecha)),
                            anulado = reader.GetBoolean(nameof(Pago.anulado))
                        });
                    }
                }
                connection.Close();
            }
        }
        return pagos;
    }


    public int Alta(Pago pago)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"INSERT INTO Pagos 
                            ({nameof(Pago.IdContrato)}, 
                             {nameof(Pago.concepto)}, 
                             {nameof(Pago.importe)}, 
                             {nameof(Pago.Fecha)}, 
                             {nameof(Pago.anulado)})
                            VALUES (@{nameof(Pago.IdContrato)}, 
                                    @{nameof(Pago.concepto)}, 
                                    @{nameof(Pago.importe)}, 
                                    @{nameof(Pago.Fecha)}, 
                                    @{nameof(Pago.anulado)});
                            SELECT LAST_INSERT_ID();";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Pago.IdContrato)}", pago.IdContrato);
                command.Parameters.AddWithValue($"@{nameof(Pago.concepto)}", pago.concepto ?? "");
                command.Parameters.AddWithValue($"@{nameof(Pago.importe)}", pago.importe);
                command.Parameters.AddWithValue($"@{nameof(Pago.Fecha)}", pago.Fecha);
                command.Parameters.AddWithValue($"@{nameof(Pago.anulado)}", pago.anulado);

                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                pago.IdPago = res;
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Pago pago)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"UPDATE Pagos
                            SET {nameof(Pago.concepto)} = @{nameof(Pago.concepto)}
                            WHERE {nameof(Pago.IdPago)} = @{nameof(Pago.IdPago)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Pago.IdPago)}", pago.IdPago);
                command.Parameters.AddWithValue($"@{nameof(Pago.concepto)}", pago.concepto ?? "");
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
            string sql = $@"UPDATE Pagos
                            SET {nameof(Pago.anulado)} = true
                            WHERE {nameof(Pago.IdPago)} = @{nameof(Pago.IdPago)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Pago.IdPago)}", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }
}
