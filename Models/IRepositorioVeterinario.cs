using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Models
{
    public interface IRepositorioVeterinario
    {
        IList<Veterinario> ObtenerTodos();
        Veterinario ObtenerPorId(int id);
        IList<Veterinario> ObtenerPorDNI(int dni);
        IList<Veterinario> ObtenerPorMatricula(string matricula);
        int Alta(Veterinario veterinario);
        int Modificacion(Veterinario veterinario);
        int Baja(int id);
        IList<Veterinario> ObtenerVeterinariosSinUsuario();
        int ObtenerCantidad();
        IList<Veterinario> ObtenerTodosPaginado(int pagina, int cantidadPorPagina);
    }
}
