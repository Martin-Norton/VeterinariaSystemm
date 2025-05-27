using System.Collections.Generic;

namespace VeterinariaSystem.Models
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        Usuario? ObtenerPorEmail(string email);
        int ModificacionConClave(Usuario u);
        int ObtenerCantidad();
        IList<Usuario> ObtenerTodosPaginado(int pagina, int tamaño);
    }
}
