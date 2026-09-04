using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class AutorService : IAutorService
    {
        private readonly List<Autores> _autores;

        public AutorService()
        {
            _autores = new List<Autores>
            {
                new Autores
                {
                    Id = 1,
                    Nombre = "Gabriel",
                    Apellido = "García Márquez",
                    Nacionalidad = "Colombiano",
                    FechaNacimiento = new DateTime(1927, 3, 6),
                    Activo = true
                },
                new Autores
                {
                    Id = 2,
                    Nombre = "Isabel",
                    Apellido = "Allende",
                    Nacionalidad = "Chilena",
                    FechaNacimiento = new DateTime(1942, 8, 2),
                    Activo = true
                },
                new Autores
                {
                    Id = 3,
                    Nombre = "Jorge Luis",
                    Apellido = "Borges",
                    Nacionalidad = "Argentino",
                    FechaNacimiento = new DateTime(1899, 8, 24),
                    Activo = false
                },
                new Autores
                {
                    Id = 4,
                    Nombre = "Mario",
                    Apellido = "Vargas Llosa",
                    Nacionalidad = "Peruano",
                    FechaNacimiento = new DateTime(1936, 3, 28),
                    Activo = true
                },
                new Autores
                {
                    Id = 5,
                    Nombre = "Julio",
                    Apellido = "Cortázar",
                    Nacionalidad = "Argentino",
                    FechaNacimiento = new DateTime(1914, 8, 26),
                    Activo = false
                }
            };
        }

        public IEnumerable<Autores> ObtenerTodos()
        {
            return _autores.OrderBy(a => a.Id);
        }

        public Autores? ObtenerPorId(int id)
        {
            return _autores.FirstOrDefault(a => a.Id == id);
        }
    }
}