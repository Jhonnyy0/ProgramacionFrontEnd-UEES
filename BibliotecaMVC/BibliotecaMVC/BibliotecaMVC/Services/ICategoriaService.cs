using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    /// <summary>
    /// Contrato del mantenimiento de Categorías (CRUD completo).
    /// El controlador depende de esta abstracción y no conoce ADO.NET
    /// ni la cadena de conexión (separación de responsabilidades).
    /// </summary>
    public interface ICategoriaService
    {
        IEnumerable<Categoria> ObtenerTodas();

        Categoria? ObtenerPorId(int id);

        int Crear(Categoria categoria);

        bool Actualizar(Categoria categoria);

        bool Eliminar(int id);
    }
}
