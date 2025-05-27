using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace VeterinariaSystem.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration)
            : base(configuration) { }

        public int Alta(Usuario u)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"INSERT INTO Usuarios
					(Nombre, Apellido, Email, Clave, Avatar, Rol)
					VALUES (@nombre, @apellido, @email, @clave, @avatar, @rol);
					SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", u.Nombre);
                    command.Parameters.AddWithValue("@apellido", u.Apellido);
                    command.Parameters.AddWithValue("@email", u.Email);
                    command.Parameters.AddWithValue("@clave", u.Clave);
                    command.Parameters.AddWithValue(
                        "@avatar",
                        u.Avatar == null ? DBNull.Value : u.Avatar
                    );
                    command.Parameters.AddWithValue("@rol", u.Rol);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    u.Id = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Usuarios WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Usuario u)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"UPDATE Usuarios SET
					Nombre=@nombre, Apellido=@apellido, Email=@email, Avatar=@avatar, Rol=@rol
					WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", u.Nombre);
                    command.Parameters.AddWithValue("@apellido", u.Apellido);
                    command.Parameters.AddWithValue("@email", u.Email);
                    command.Parameters.AddWithValue(
                        "@avatar",
                        u.Avatar == null ? DBNull.Value : u.Avatar
                    );
                    command.Parameters.AddWithValue("@rol", u.Rol);
                    command.Parameters.AddWithValue("@id", u.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int ModificacionConClave(Usuario u)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"UPDATE Usuarios SET
							Nombre=@nombre, Apellido=@apellido, Email=@email, Avatar=@avatar, Rol=@rol, Clave=@clave
							WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", u.Nombre);
                    command.Parameters.AddWithValue("@apellido", u.Apellido);
                    command.Parameters.AddWithValue("@email", u.Email);
                    command.Parameters.AddWithValue(
                        "@avatar",
                        u.Avatar == null ? DBNull.Value : u.Avatar
                    );
                    command.Parameters.AddWithValue("@rol", u.Rol);
                    command.Parameters.AddWithValue("@clave", u.Clave);
                    command.Parameters.AddWithValue("@id", u.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Usuario> ObtenerTodos()
        {
            IList<Usuario> res = new List<Usuario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT Id, Nombre, Apellido, Email, Clave, Avatar, Rol FROM Usuarios";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Usuario u = new Usuario
                        {
                            Id = reader.GetInt32("Id"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Email = reader.GetString("Email"),
                            Clave = reader.GetString("Clave"),
                            Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar"))
                                ? null
                                : reader.GetString("Avatar"),
                            Rol = reader.GetInt32("Rol"),
                        };
                        res.Add(u);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Usuario> ObtenerTodosPaginado(int pagina, int tamaño)
        {
            IList<Usuario> res = new List<Usuario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, Nombre, Apellido, Email, Clave, Avatar, Rol 
                       FROM Usuarios 
                       LIMIT @offset, @tamaño";
                using (var command = new MySqlCommand(sql, connection))
                {
                    int offset = (pagina - 1) * tamaño;
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@tamaño", tamaño);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Usuario u = new Usuario
                        {
                            Id = reader.GetInt32("Id"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Email = reader.GetString("Email"),
                            Clave = reader.GetString("Clave"),
                            Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar"))
                                ? null
                                : reader.GetString("Avatar"),
                            Rol = reader.GetInt32("Rol"),
                        };
                        res.Add(u);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Usuarios";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    int cantidad = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                    return cantidad;
                }
            }
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? u = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    $"SELECT Id, Nombre, Apellido, Email, Clave, Avatar, Rol FROM Usuarios WHERE Id=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        u = new Usuario
                        {
                            Id = reader.GetInt32("Id"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Email = reader.GetString("Email"),
                            Clave = reader.GetString("Clave"),
                            Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar"))
                                ? null
                                : reader.GetString("Avatar"),
                            Rol = reader.GetInt32("Rol"),
                        };
                    }
                    connection.Close();
                }
            }
            return u;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? u = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    $"SELECT Id, Nombre, Apellido, Email, Clave, Avatar, Rol FROM Usuarios WHERE Email=@email";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        u = new Usuario
                        {
                            Id = reader.GetInt32("Id"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Email = reader.GetString("Email"),
                            Clave = reader.GetString("Clave"),
                            Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar"))
                                ? null
                                : reader.GetString("Avatar"),
                            Rol = reader.GetInt32("Rol"),
                        };
                    }
                    connection.Close();
                }
            }
            return u;
        }
    }
}
