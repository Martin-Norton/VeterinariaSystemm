using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace VeterinariaSystem.Models
{
    public class RepositorioDueno : RepositorioBase, IRepositorioDueno
    {
        public RepositorioDueno(IConfiguration configuration)
            : base(configuration) { }

        public int Alta(Dueno p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"INSERT INTO Dueno
                    (DNI, Nombre, Apellido, Telefono, Email, Estado)
                    VALUES (@dni, @nombre, @apellido, @telefono, @email, @estado);";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", p.DNI);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@estado", p.Estado);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.Id = res;
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
                string sql = @"UPDATE Dueno SET Estado = 0 WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Dueno p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"UPDATE Dueno SET
                    DNI = @dni,
                    Nombre = @nombre,
                    Apellido = @apellido,
                    Telefono = @telefono,
                    Email = @email
                    WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", p.Id);
                    command.Parameters.AddWithValue("@dni", p.DNI);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Dueno> ObtenerTodos()
        {
            IList<Dueno> lista = new List<Dueno>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, DNI, Nombre, Apellido, Telefono, Email FROM Dueno WHERE Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dueno dueno = new Dueno
                            {
                                Id = reader.GetInt32(0),
                                DNI = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5),
                            };
                            lista.Add(dueno);
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        public IList<Dueno> ObtenerTodosPaginado(int pagina, int tamañoPagina)
        {
            IList<Dueno> lista = new List<Dueno>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, DNI, Nombre, Apellido, Telefono, Email
                       FROM Dueno
                       WHERE Estado = 1
                       LIMIT @offset, @limit;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    int offset = (pagina - 1) * tamañoPagina;
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@limit", tamañoPagina);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dueno dueno = new Dueno
                            {
                                Id = reader.GetInt32(0),
                                DNI = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5),
                            };
                            lista.Add(dueno);
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        public int ContarDueno()
        {
            int total = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Dueno WHERE Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    total = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return total;
        }

        public Dueno ObtenerPorId(int id)
        {
            Dueno dueno = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, DNI, Nombre, Apellido, Telefono, Email FROM Dueno WHERE Id = @id AND Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dueno = new Dueno
                            {
                                Id = reader.GetInt32(0),
                                DNI = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5),
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return dueno;
        }

        public Dueno ObtenerPorEmail(string email)
        {
            Dueno dueno = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, DNI, Nombre, Apellido, Telefono, Email FROM Dueno WHERE Email = @email AND Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dueno = new Dueno
                            {
                                Id = reader.GetInt32(0),
                                DNI = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5),
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return dueno;
        }

        public Dueno ObtenerPorDni(int dni)
        {
            Dueno dueno = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, DNI, Nombre, Apellido, Telefono, Email FROM Dueno WHERE DNI = @dni AND Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dueno = new Dueno
                            {
                                Id = reader.GetInt32(0),
                                DNI = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5),
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return dueno;
        }

        //zona usuario
        public IList<Dueno> ObtenerDuenosSinUsuario()
        {
            var duenos = new List<Dueno>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var query =
                    @"SELECT d.Id, d.DNI, d.Nombre, d.Apellido, d.Telefono, d.Email, d.Estado
                      FROM Dueno d
                      LEFT JOIN Usuarios u ON d.Id = u.Id_Dueno
                      WHERE u.Id IS NULL AND d.Estado = 1";

                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        duenos.Add(
                            new Dueno
                            {
                                Id = reader.GetInt32("Id"),
                                DNI = reader.GetInt32("DNI"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono"))
                                    ? null
                                    : reader.GetString("Telefono"),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                                    ? null
                                    : reader.GetString("Email"),
                                Estado = reader.GetInt32("Estado"),
                            }
                        );
                    }
                }
            }
            return duenos;
        }

        public Dueno BuscarPorDni(int dni)
        {
            Dueno dueno = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var query =
                    @"SELECT d.Id, d.DNI, d.Nombre, d.Apellido, d.Telefono, d.Email, d.Estado
                      FROM Dueno d
                      LEFT JOIN Usuarios u ON d.Id = u.Id_Dueno
                      WHERE d.DNI = @dni AND u.Id IS NULL AND d.Estado = 1";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        dueno = new Dueno
                        {
                            Id = reader.GetInt32("Id"),
                            DNI = reader.GetInt32("DNI"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono"))
                                ? null
                                : reader.GetString("Telefono"),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                                ? null
                                : reader.GetString("Email"),
                            Estado = reader.GetInt32("Estado"),
                        };
                    }
                }
            }
            return dueno;
        }

        //FinZona Usuario
    }
}
