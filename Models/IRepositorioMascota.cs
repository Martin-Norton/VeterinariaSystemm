using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Models
{
    public interface IRepositorioMascota
    {
        IList<Mascota> ObtenerTodos();
        Mascota ObtenerPorId(int id);
        int ObtenerCantidadPorDueno(int idDueno);
        IList<Mascota> ObtenerPorDuenoPaginado(int idDueno, int pagina, int tamaño);
        IList<Mascota> ObtenerPorDuenoo(int idDueno);
        int Alta(Mascota mascota);
        int Modificacion(Mascota mascota);
        int Baja(int id);
        int ObtenerCantidadMascotasActivas();
        IList<Mascota> ObtenerTodosPaginado(int pagina, int tamaño);
    }
}
