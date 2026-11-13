using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public interface ILibroService
    {
        IEnumerable<Libro> ObtenerTodos(string? buscar);

        IEnumerable<Libro> ObtenerPorAutor(string? autor);

        IEnumerable<string> ObtenerAutores();

        Libro? ObtenerPorId(int id);

        int Crear(Libro libro);

        bool Actualizar(Libro libro);

        bool Eliminar(int id);
    }
}
