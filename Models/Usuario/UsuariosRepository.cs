using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace _Net.Models
{
    public class UsuariosRepository
    {
        private readonly string _connectionString;

        public UsuariosRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection");
        }

        public List<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"
                SELECT idUsuario, email, password, nombre, rol, activo, avatar, IdPropietario
                FROM usuarios;";
            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(MapUsuario(r));
            }
            return lista;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? u = null;
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"
                SELECT idUsuario, email, password, nombre, rol, activo, avatar, IdPropietario
                FROM usuarios
                WHERE idUsuario=@Id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (r.Read()) u = MapUsuario(r);
            return u;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? u = null;
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"
                SELECT idUsuario, email, password, nombre, rol, activo, avatar, IdPropietario
                FROM usuarios
                WHERE email=@Email AND activo=1
                LIMIT 1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (r.Read()) u = MapUsuario(r);
            return u;
        }

        public int Alta(Usuario u)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO usuarios (email, password, nombre, rol, activo, avatar, IdPropietario)
                VALUES (@Email, @Password, @Nombre, @Rol, @Activo, @Avatar, @IdPropietario);
                SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@Nombre", (object?)u.Nombre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
            cmd.Parameters.AddWithValue("@Activo", u.Activo);
            cmd.Parameters.AddWithValue("@Avatar", (object?)u.Avatar ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdPropietario", (object?)u.IdPropietario ?? DBNull.Value);

            conn.Open();
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public int Modificar(Usuario u)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"
                UPDATE usuarios
                SET email=@Email,
                    password=@Password,
                    nombre=@Nombre,
                    rol=@Rol,
                    activo=@Activo,
                    avatar=@Avatar,
                    IdPropietario=@IdPropietario
                WHERE idUsuario=@IdUsuario;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@Nombre", (object?)u.Nombre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
            cmd.Parameters.AddWithValue("@Activo", u.Activo);
            cmd.Parameters.AddWithValue("@Avatar", (object?)u.Avatar ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdPropietario", (object?)u.IdPropietario ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdUsuario", u.IdUsuario);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int BajaLogica(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = "UPDATE usuarios SET activo=0 WHERE idUsuario=@Id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // --- Helpers útiles para la API de propietarios ---

        public int ActualizarPassword(int idUsuario, string nuevoPasswordPlano)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"UPDATE usuarios SET password=@Pass WHERE idUsuario=@Id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Pass", nuevoPasswordPlano); // TODO: usar hash (BCrypt) si querés
            cmd.Parameters.AddWithValue("@Id", idUsuario);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int VincularPropietario(int idUsuario, int idPropietario)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"UPDATE usuarios SET IdPropietario=@Pid WHERE idUsuario=@Uid;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Pid", idPropietario);
            cmd.Parameters.AddWithValue("@Uid", idUsuario);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        private static Usuario MapUsuario(IDataRecord r) => new Usuario
        {
            IdUsuario = r.GetInt32(r.GetOrdinal("idUsuario")),
            Email = r.GetString(r.GetOrdinal("email")),
            Password = r.GetString(r.GetOrdinal("password")),
            Nombre = r.IsDBNull(r.GetOrdinal("nombre")) ? null : r.GetString(r.GetOrdinal("nombre")),
            Rol = r.GetString(r.GetOrdinal("rol")),
            Activo = r.GetBoolean(r.GetOrdinal("activo")),
            Avatar = r.IsDBNull(r.GetOrdinal("avatar")) ? null : r.GetString(r.GetOrdinal("avatar")),
            IdPropietario = r.IsDBNull(r.GetOrdinal("IdPropietario")) ? (int?)null : r.GetInt32(r.GetOrdinal("IdPropietario"))
        };
    }
}
