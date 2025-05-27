using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace VeterinariaSystem.Models
{
    public class RepositorioMascota : RepositorioBase, IRepositorioMascota
    {
        public RepositorioMascota(IConfiguration configuration)
            : base(configuration) { }

        public int Alta(Mascota p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"INSERT INTO Mascota
                    (Nombre, Especie, Raza, Edad, Peso, Sexo, Id_Dueno, Estado)
                    VALUES (@nombre, @especie, @raza, @edad, @peso, @sexo, @id_dueno, @estado);";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@especie", p.Especie);
                    command.Parameters.AddWithValue("@raza", p.Raza);
                    command.Parameters.AddWithValue("@edad", p.Edad);
                    command.Parameters.AddWithValue("@peso", p.Peso);
                    command.Parameters.AddWithValue("@sexo", p.Sexo);
                    command.Parameters.AddWithValue("@id_dueno", p.Id_Dueno);
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
                string sql = @"UPDATE Mascota SET Estado = 0 WHERE Id = @id;";
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

        public int Modificacion(Mascota p)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"UPDATE Mascota SET
                    Nombre = @nombre,
                    Especie = @especie,
                    Raza = @raza,
                    Edad = @edad,
                    Peso = @peso,
                    Sexo = @sexo,
                    Id_Dueno = @id_dueno,
                    Estado = @estado
                    WHERE Id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", p.Id);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@especie", p.Especie);
                    command.Parameters.AddWithValue("@raza", p.Raza);
                    command.Parameters.AddWithValue("@edad", p.Edad);
                    command.Parameters.AddWithValue("@peso", p.Peso);
                    command.Parameters.AddWithValue("@sexo", p.Sexo);
                    command.Parameters.AddWithValue("@id_dueno", p.Id_Dueno);
                    command.Parameters.AddWithValue("@estado", p.Estado);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public Mascota ObtenerPorId(int id)
        {
            Mascota p = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT m.Id, m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno, m.Estado,
                                    d.Nombre AS NombreDueno, d.Apellido AS ApellidoDueno
                            FROM Mascota m
                            JOIN Dueno d ON m.Id_Dueno = d.Id
                            WHERE m.Id = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Mascota
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Especie = reader.GetString(2),
                                Raza = reader.GetString(3),
                                Edad = reader.GetInt32(4),
                                Peso = reader.GetInt32(5),
                                Sexo = reader.GetString(6),
                                Id_Dueno = reader.GetInt32(7),
                                Estado = reader.GetInt32(8),
                                Dueno = new Dueno
                                {
                                    Nombre = reader.GetString(9),
                                    Apellido = reader.GetString(10),
                                },
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public IList<Mascota> ObtenerTodos()
        {
            IList<Mascota> lista = new List<Mascota>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT m.Id, m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno, m.Estado,
                                    d.Nombre AS NombreDueno, d.Apellido AS ApellidoDueno
                            FROM Mascota m
                            JOIN Dueno d ON m.Id_Dueno = d.Id
                            WHERE m.Estado = 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Mascota p = new Mascota
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Especie = reader.GetString(2),
                                Raza = reader.GetString(3),
                                Edad = reader.GetInt32(4),
                                Peso = reader.GetInt32(5),
                                Sexo = reader.GetString(6),
                                Id_Dueno = reader.GetInt32(7),
                                Estado = reader.GetInt32(8),
                                Dueno = new Dueno
                                {
                                    Nombre = reader.GetString(9),
                                    Apellido = reader.GetString(10),
                                },
                            };
                            lista.Add(p);
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        public IList<Mascota> ObtenerTodosPaginado(int pagina, int tamaño)
        {
            IList<Mascota> lista = new List<Mascota>();
            int offset = (pagina - 1) * tamaño;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"
            SELECT m.Id, m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno, m.Estado,
                   d.Nombre AS NombreDueno, d.Apellido AS ApellidoDueno
            FROM Mascota m
            JOIN Dueno d ON m.Id_Dueno = d.Id
            WHERE m.Estado = 1
            ORDER BY m.Nombre
            LIMIT @tamaño OFFSET @offset;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tamaño", tamaño);
                    command.Parameters.AddWithValue("@offset", offset);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Mascota p = new Mascota
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Especie = reader.GetString(2),
                                Raza = reader.GetString(3),
                                Edad = reader.GetInt32(4),
                                Peso = reader.GetInt32(5),
                                Sexo = reader.GetString(6),
                                Id_Dueno = reader.GetInt32(7),
                                Estado = reader.GetInt32(8),
                                Dueno = new Dueno
                                {
                                    Nombre = reader.GetString(9),
                                    Apellido = reader.GetString(10),
                                },
                            };
                            lista.Add(p);
                        }
                    }
                }
            }

            return lista;
        }

        public int ObtenerCantidadMascotasActivas()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Mascota WHERE Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public IList<Mascota> ObtenerPorDuenoo(int idDueno)
        {
            IList<Mascota> lista = new List<Mascota>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    @"SELECT m.Id, m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno, m.Estado,
                                    d.Nombre AS NombreDueno, d.Apellido AS ApellidoDueno
                            FROM Mascota m
                            JOIN Dueno d ON m.Id_Dueno = d.Id
                            WHERE m.Id_Dueno = @idDueno AND m.Estado = 1;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idDueno", idDueno);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Mascota p = new Mascota
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Especie = reader.GetString(2),
                                Raza = reader.GetString(3),
                                Edad = reader.GetInt32(4),
                                Peso = reader.GetInt32(5),
                                Sexo = reader.GetString(6),
                                Id_Dueno = reader.GetInt32(7),
                                Estado = reader.GetInt32(8),
                                Dueno = new Dueno
                                {
                                    Nombre = reader.GetString(9),
                                    Apellido = reader.GetString(10),
                                },
                            };
                            lista.Add(p);
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        public IList<Mascota> ObtenerPorDuenoPaginado(int idDueno, int pagina, int tamaño)
        {
            IList<Mascota> lista = new List<Mascota>();
            using (var connection = new MySqlConnection(connectionString))
            {
                int offset = (pagina - 1) * tamaño;

                string sql =
                    @"
            SELECT m.Id, m.Nombre, m.Especie, m.Raza, m.Edad, m.Peso, m.Sexo, m.Id_Dueno, m.Estado,
                   d.Nombre AS NombreDueno, d.Apellido AS ApellidoDueno
            FROM Mascota m
            JOIN Dueno d ON m.Id_Dueno = d.Id
            WHERE m.Id_Dueno = @idDueno AND m.Estado = 1
            ORDER BY m.Nombre
            LIMIT @tamaño OFFSET @offset;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idDueno", idDueno);
                    command.Parameters.AddWithValue("@tamaño", tamaño);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Mascota m = new Mascota
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Especie = reader.GetString(2),
                                Raza = reader.GetString(3),
                                Edad = reader.GetInt32(4),
                                Peso = reader.GetInt32(5),
                                Sexo = reader.GetString(6),
                                Id_Dueno = reader.GetInt32(7),
                                Estado = reader.GetInt32(8),
                                Dueno = new Dueno
                                {
                                    Nombre = reader.GetString(9),
                                    Apellido = reader.GetString(10),
                                },
                            };
                            lista.Add(m);
                        }
                    }
                }
            }
            return lista;
        }

        public int ObtenerCantidadPorDueno(int idDueno)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql =
                    "SELECT COUNT(*) FROM Mascota WHERE Id_Dueno = @idDueno AND Estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idDueno", idDueno);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
    }
}
