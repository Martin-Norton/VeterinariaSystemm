using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace VeterinariaSystem.Models
{
    public class RepositorioVeterinario : RepositorioBase, IRepositorioVeterinario
    {
        public RepositorioVeterinario(IConfiguration configuration)
            : base(configuration) { }

        public int Alta(Veterinario p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"INSERT INTO Veterinario
                    (Nombre, Apellido, DNI, Matricula, Email)
                    VALUES (@nombre, @apellido, @dni, @matricula, @email);";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.DNI);
                    command.Parameters.AddWithValue("@matricula", p.Matricula);
                    command.Parameters.AddWithValue("@email", p.Email);
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
                string sql = @"UPDATE Veterinario SET Estado = 0 WHERE Id = @id;";
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

        public int Modificacion(Veterinario p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"UPDATE Veterinario SET
                    Nombre = @nombre,
                    Apellido = @apellido,
                    DNI = @dni,
                    Matricula = @matricula,
                    Email = @email
                    WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.DNI);
                    command.Parameters.AddWithValue("@matricula", p.Matricula);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@id", p.Id);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Veterinario> ObtenerTodos()
        {
            IList<Veterinario> lista = new List<Veterinario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, Nombre, Apellido, DNI, Matricula, Email, Estado FROM Veterinario WHERE Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Veterinario p = new Veterinario
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                DNI = reader.GetInt32(3),
                                Matricula = reader.GetString(4),
                                Email = reader.GetString(5),
                                Estado = reader.GetInt32(6),
                            };
                            lista.Add(p);
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        public IList<Veterinario> ObtenerTodosPaginado(int pagina, int cantidadPorPagina)
        {
            IList<Veterinario> lista = new List<Veterinario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                int offset = (pagina - 1) * cantidadPorPagina;
                string sql =
                    @"SELECT Id, Nombre, Apellido, DNI, Matricula, Email, Estado 
                       FROM Veterinario 
                       WHERE Estado = 1 
                       LIMIT @cantidad OFFSET @offset;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@cantidad", cantidadPorPagina);
                    command.Parameters.AddWithValue("@offset", offset);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Veterinario p = new Veterinario
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                DNI = reader.GetInt32(3),
                                Matricula = reader.GetString(4),
                                Email = reader.GetString(5),
                                Estado = reader.GetInt32(6),
                            };
                            lista.Add(p);
                        }
                    }
                }
            }
            return lista;
        }

        public int ObtenerCantidad()
        {
            int total = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Veterinario WHERE Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    total = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return total;
        }

        public Veterinario ObtenerPorId(int id)
        {
            Veterinario p = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, Nombre, Apellido, DNI, Matricula, Email, Estado FROM Veterinario WHERE Id = @id AND Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Veterinario
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                DNI = reader.GetInt32(3),
                                Matricula = reader.GetString(4),
                                Email = reader.GetString(5),
                                Estado = reader.GetInt32(6),
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public IList<Veterinario> ObtenerPorDNI(int dni)
        {
            IList<Veterinario> lista = new List<Veterinario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, Nombre, Apellido, DNI, Matricula, Email, Estado FROM Veterinario WHERE DNI = @dni AND Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Veterinario p = new Veterinario
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                DNI = reader.GetInt32(3),
                                Matricula = reader.GetString(4),
                                Email = reader.GetString(5),
                                Estado = reader.GetInt32(6),
                            };
                            lista.Add(p);
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        public IList<Veterinario> ObtenerPorMatricula(string matricula)
        {
            IList<Veterinario> lista = new List<Veterinario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, Nombre, Apellido, DNI, Matricula, Email, Estado FROM Veterinario WHERE Matricula = @matricula AND Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@matricula", matricula);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Veterinario p = new Veterinario
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                DNI = reader.GetInt32(3),
                                Matricula = reader.GetString(4),
                                Email = reader.GetString(5),
                                Estado = reader.GetInt32(6),
                            };
                            lista.Add(p);
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        //zona usuario
        public IList<Veterinario> ObtenerVeterinariosSinUsuario()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var veterinarios = new List<Veterinario>();
                var query =
                    @"SELECT v.Id, v.Nombre, v.Apellido, v.DNI, v.Matricula, v.Email, v.Estado
                            FROM Veterinario v
                            LEFT JOIN Usuarios u ON v.Id = u.Id_Veterinario
                            WHERE u.Id IS NULL AND v.Estado = 1";

                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        veterinarios.Add(
                            new Veterinario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetInt32("DNI"),
                                Matricula = reader.GetString("Matricula"),
                                Email = reader.GetString("Email"),
                                Estado = reader.GetInt32("Estado"),
                            }
                        );
                    }
                }
                return veterinarios;
            }
        }
        //FinZona Usuario
    }
}
