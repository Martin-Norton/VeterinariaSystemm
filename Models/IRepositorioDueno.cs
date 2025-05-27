using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Models
{
    public interface IRepositorioDueno
    {
        IList<Dueno> ObtenerTodos();
        Dueno ObtenerPorId(int id);
        Dueno ObtenerPorEmail(string email);
        Dueno ObtenerPorDni(int dni);
        Dueno BuscarPorDni(int dni);
        IList<Dueno> ObtenerDuenosSinUsuario();
        int Alta(Dueno dueno);
        int Modificacion(Dueno dueno);
        int Baja(int id);
        IList<Dueno> ObtenerTodosPaginado(int pagina, int tamañoPagina);
        int ContarDueno();
    }
}
