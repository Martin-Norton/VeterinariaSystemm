using VeterinariaSystem.Models;

public interface IRepositorioConsulta
{
    Consulta ObtenerPorId(int id);
    IList<Consulta> ObtenerTodos();
    int Alta(Consulta consulta);
    int Modificacion(Consulta consulta);
    int Baja(int id);
    IList<Consulta> ObtenerPaginadas(int pagina, int tamaño);
    int ObtenerCantidad();
    int ObtenerCantidadPorFechas(DateTime fechaInicio, DateTime fechaFin);
    IList<Consulta> BuscarPorFechasPaginado(
        DateTime fechaInicio,
        DateTime fechaFin,
        int pagina,
        int tamaño
    );
    int ObtenerCantidadPorVeterinario(int idVeterinario);
    IList<Consulta> BuscarPorVeterinarioPaginado(int idVeterinario, int pagina, int tamaño);
    int ObtenerCantidadPorMascota(int idMascota);
    IList<Consulta> BuscarPorMascotaPaginado(int idMascota, int pagina, int tamaño);
    IList<Consulta> ObtenerHistoriaClinica(
        int idMascota,
        int pagina,
        int tamanoPagina,
        out int totalRegistros
    );
}
