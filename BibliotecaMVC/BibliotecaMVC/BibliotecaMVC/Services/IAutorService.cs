using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{

    public interface IAutorService
    {
        IEnumerable<Autores> ObtenerTodos();

        Autores? ObtenerPorId(int id);
    }
}
