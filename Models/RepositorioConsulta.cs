using MySql.Data.MySqlClient;
using VeterinariaSystem.Models;

public class RepositorioConsulta : RepositorioBase, IRepositorioConsulta
{
    public RepositorioConsulta(IConfiguration configuration)
        : base(configuration) { }

    public int Alta(Consulta consulta)
    {
        int id = 0;
        using var connection = new MySqlConnection(connectionString);
        var sql =
            @"INSERT INTO Consulta (Id_Turno, Fecha, Motivo, Diagnostico, Tratamiento, ArchivoAdjunto, Id_Mascota, Id_Veterinario)
                    VALUES (@id_turno, @fecha, @motivo, @diagnostico, @tratamiento, @archivo, @id_mascota, @id_veterinario);
                    SELECT LAST_INSERT_ID();";
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id_turno", consulta.Id_Turno);
        command.Parameters.AddWithValue("@fecha", consulta.Fecha);
        command.Parameters.AddWithValue("@motivo", consulta.Motivo);
        command.Parameters.AddWithValue("@diagnostico", consulta.Diagnostico ?? "");
        command.Parameters.AddWithValue("@tratamiento", consulta.Tratamiento ?? "");
        command.Parameters.AddWithValue("@archivo", consulta.ArchivoAdjunto ?? "");
        Console.WriteLine("Ruta archivo adjunto: " + consulta.ArchivoAdjunto);
        command.Parameters.AddWithValue("@id_mascota", consulta.Id_Mascota);
        command.Parameters.AddWithValue("@id_veterinario", consulta.Id_Veterinario);
        connection.Open();
        id = Convert.ToInt32(command.ExecuteScalar());
        connection.Close();
        return id;
    }

    public int Baja(int id)
    {
        using var connection = new MySqlConnection(connectionString);
        var sql = "DELETE FROM Consulta WHERE Id = @id";
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public Consulta ObtenerPorId(int id)
    {
        Consulta consulta = null;
        using var connection = new MySqlConnection(connectionString);
        var sql =
            @"SELECT Id, Id_Turno, Fecha, Motivo, Diagnostico, Tratamiento, ArchivoAdjunto, Id_Mascota, Id_Veterinario 
                    FROM Consulta WHERE Id = @id";
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            consulta = new Consulta
            {
                Id = reader.GetInt32(0),
                Id_Turno = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                Fecha = reader.GetDateTime(2),
                Motivo = reader.GetString(3),
                Diagnostico = reader.IsDBNull(4) ? null : reader.GetString(4),
                Tratamiento = reader.IsDBNull(5) ? null : reader.GetString(5),
                ArchivoAdjunto = reader.IsDBNull(6) ? null : reader.GetString(6),
                Id_Mascota = reader.GetInt32(7),
                Id_Veterinario = reader.GetInt32(8),
            };
        }
        connection.Close();
        return consulta;
    }

    public IList<Consulta> ObtenerTodos()
    {
        var lista = new List<Consulta>();
        using var connection = new MySqlConnection(connectionString);
        var sql =
            "SELECT Id, Id_Turno, Fecha, Motivo, Diagnostico, Tratamiento, ArchivoAdjunto, Id_Mascota, Id_Veterinario FROM Consulta";
        using var command = new MySqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(
                new Consulta
                {
                    Id = reader.GetInt32(0),
                    Id_Turno = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    Fecha = reader.GetDateTime(2),
                    Motivo = reader.GetString(3),
                    Diagnostico = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Tratamiento = reader.IsDBNull(5) ? null : reader.GetString(5),
                    ArchivoAdjunto = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Id_Mascota = reader.GetInt32(7),
                    Id_Veterinario = reader.GetInt32(8),
                }
            );
        }
        connection.Close();
        return lista;
    }

    public int Modificacion(Consulta consulta)
    {
        using var connection = new MySqlConnection(connectionString);
        var sql =
            @"UPDATE Consulta SET Id_Turno=@id_turno, Fecha=@fecha, Motivo=@motivo, Diagnostico=@diagnostico,
                    Tratamiento=@tratamiento, ArchivoAdjunto=@archivo, Id_Mascota=@id_mascota, Id_Veterinario=@id_veterinario 
                    WHERE Id=@id";
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", consulta.Id);
        command.Parameters.AddWithValue("@id_turno", consulta.Id_Turno);
        command.Parameters.AddWithValue("@fecha", consulta.Fecha);
        command.Parameters.AddWithValue("@motivo", consulta.Motivo);
        command.Parameters.AddWithValue("@diagnostico", consulta.Diagnostico ?? "");
        command.Parameters.AddWithValue("@tratamiento", consulta.Tratamiento ?? "");
        command.Parameters.AddWithValue("@archivo", consulta.ArchivoAdjunto ?? "");
        command.Parameters.AddWithValue("@id_mascota", consulta.Id_Mascota);
        command.Parameters.AddWithValue("@id_veterinario", consulta.Id_Veterinario);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public IList<Consulta> ObtenerPaginadas(int pagina, int tamaño)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var consultas = new List<Consulta>();
            var offset = (pagina - 1) * tamaño;
            var sql = @"SELECT * FROM Consulta ORDER BY Fecha DESC LIMIT @tamaño OFFSET @offset";
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@tamaño", tamaño);
            command.Parameters.AddWithValue("@offset", offset);
            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var consulta = new Consulta
                {
                    Id = reader.GetInt32("Id"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Motivo = reader.GetString("Motivo"),
                    Diagnostico = reader.GetString("Diagnostico"),
                    Tratamiento = reader.GetString("Tratamiento"),
                    ArchivoAdjunto =
                        reader["ArchivoAdjunto"] != DBNull.Value
                            ? reader.GetString("ArchivoAdjunto")
                            : null,
                    Id_Mascota = reader.GetInt32("Id_Mascota"),
                    Id_Veterinario = reader.GetInt32("Id_Veterinario"),
                    Id_Turno =
                        reader["Id_Turno"] != DBNull.Value
                            ? reader.GetInt32("Id_Turno")
                            : (int?)null,
                };
                consultas.Add(consulta);
            }
            return consultas;
        }
    }

    public int ObtenerCantidad()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT COUNT(*) FROM Consulta";
            var command = new MySqlCommand(sql, connection);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }
    }

    //zona busquedas
    public IList<Consulta> BuscarPorMascotaPaginado(int idMascota, int pagina, int tamaño)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var consultas = new List<Consulta>();
            var offset = (pagina - 1) * tamaño;
            var sql =
                @"
                SELECT * FROM Consulta
                WHERE Id_Mascota = @idMascota
                ORDER BY Fecha DESC
                LIMIT @tamaño OFFSET @offset;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idMascota", idMascota);
                command.Parameters.AddWithValue("@tamaño", tamaño);
                command.Parameters.AddWithValue("@offset", offset);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        consultas.Add(MapConsulta(reader));
                    }
                }
            }
            return consultas;
        }
    }

    public int ObtenerCantidadPorMascota(int idMascota)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT COUNT(*) FROM Consulta WHERE Id_Mascota = @idMascota;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idMascota", idMascota);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }

    public IList<Consulta> BuscarPorVeterinarioPaginado(int idVeterinario, int pagina, int tamaño)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var consultas = new List<Consulta>();
            var offset = (pagina - 1) * tamaño;
            var sql =
                @"
                SELECT * FROM Consulta
                WHERE Id_Veterinario = @idVeterinario
                ORDER BY Fecha DESC
                LIMIT @tamaño OFFSET @offset;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idVeterinario", idVeterinario);
                command.Parameters.AddWithValue("@tamaño", tamaño);
                command.Parameters.AddWithValue("@offset", offset);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        consultas.Add(MapConsulta(reader));
                    }
                }
            }
            return consultas;
        }
    }

    public int ObtenerCantidadPorVeterinario(int idVeterinario)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT COUNT(*) FROM Consulta WHERE Id_Veterinario = @idVeterinario;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idVeterinario", idVeterinario);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }

    public IList<Consulta> BuscarPorFechasPaginado(
        DateTime fechaInicio,
        DateTime fechaFin,
        int pagina,
        int tamaño
    )
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var consultas = new List<Consulta>();
            var offset = (pagina - 1) * tamaño;
            var sql =
                @"
                SELECT * FROM Consulta
                WHERE Fecha >= @fechaInicio AND Fecha <= @fechaFin
                ORDER BY Fecha DESC
                LIMIT @tamaño OFFSET @offset;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@fechaInicio", fechaInicio.Date);
                command.Parameters.AddWithValue(
                    "@fechaFin",
                    fechaFin.Date.AddDays(1).AddSeconds(-1)
                );
                command.Parameters.AddWithValue("@tamaño", tamaño);
                command.Parameters.AddWithValue("@offset", offset);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        consultas.Add(MapConsulta(reader));
                    }
                }
            }
            return consultas;
        }
    }

    public int ObtenerCantidadPorFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql =
                "SELECT COUNT(*) FROM Consulta WHERE Fecha >= @fechaInicio AND Fecha <= @fechaFin;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@fechaInicio", fechaInicio.Date);
                command.Parameters.AddWithValue(
                    "@fechaFin",
                    fechaFin.Date.AddDays(1).AddSeconds(-1)
                );
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }

    private Consulta MapConsulta(MySqlDataReader reader)
    {
        return new Consulta
        {
            Id = reader.GetInt32("Id"),
            Fecha = reader.GetDateTime("Fecha"),
            Motivo = reader.GetString("Motivo"),
            Diagnostico = reader.GetString("Diagnostico"),
            Tratamiento = reader.GetString("Tratamiento"),
            ArchivoAdjunto =
                reader["ArchivoAdjunto"] != DBNull.Value
                    ? reader.GetString("ArchivoAdjunto")
                    : null,
            Id_Mascota = reader.GetInt32("Id_Mascota"),
            Id_Veterinario = reader.GetInt32("Id_Veterinario"),
            Id_Turno =
                reader["Id_Turno"] != DBNull.Value ? reader.GetInt32("Id_Turno") : (int?)null,
        };
    }

    //fin zona busquedas

    //Zona Dueno
    public IList<Consulta> ObtenerHistoriaClinica(
        int idMascota,
        int pagina,
        int tamanoPagina,
        out int totalRegistros
    )
    {
        var lista = new List<Consulta>();
        totalRegistros = 0;

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string sqlCount = "SELECT COUNT(*) FROM Consulta WHERE Id_Mascota = @idMascota;";
            using (var countCmd = new MySqlCommand(sqlCount, connection))
            {
                countCmd.Parameters.AddWithValue("@idMascota", idMascota);
                totalRegistros = Convert.ToInt32(countCmd.ExecuteScalar());
            }
            string sql =
                @"
            SELECT Fecha, Motivo, Diagnostico, Tratamiento
            FROM Consulta
            WHERE Id_Mascota = @idMascota
            ORDER BY Id DESC
            LIMIT @limit OFFSET @offset;";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idMascota", idMascota);
                command.Parameters.AddWithValue("@limit", tamanoPagina);
                command.Parameters.AddWithValue("@offset", (pagina - 1) * tamanoPagina);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var consulta = new Consulta
                        {
                            Fecha = reader.GetDateTime(0),
                            Motivo = reader.GetString(1),
                            Diagnostico = reader.GetString(2),
                            Tratamiento = reader.GetString(3),
                        };
                        lista.Add(consulta);
                    }
                }
            }
        }

        return lista;
    }

    //Fin zona Dueno
}
