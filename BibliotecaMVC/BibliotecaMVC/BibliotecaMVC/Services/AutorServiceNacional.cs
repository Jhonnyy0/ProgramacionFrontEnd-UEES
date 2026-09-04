using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class AutorServiceNacional : IAutorService
    {
        private readonly List<Autores> _autores;

        public AutorServiceNacional()
        {
            _autores = new List<Autores>
            {
                new Autores
                {
                    Id = 1,
                    Nombre = "Salvador",
                    Apellido = "Salazar Arrué (Salarrué)",
                    Nacionalidad = "Salvadoreño",
                    FechaNacimiento = new DateTime(1899, 10, 22),
                    Activo = true
                },
                new Autores
                {
                    Id = 2,
                    Nombre = "Claudia",
                    Apellido = "Lars",
                    Nacionalidad = "Salvadoreña",
                    FechaNacimiento = new DateTime(1899, 12, 20),
                    Activo = true
                },
                new Autores
                {
                    Id = 3,
                    Nombre = "Alfredo",
                    Apellido = "Espino",
                    Nacionalidad = "Salvadoreño",
                    FechaNacimiento = new DateTime(1900, 1, 8),
                    Activo = false
                },
                new Autores
                {
                    Id = 4,
                    Nombre = "Manlio",
                    Apellido = "Argueta",
                    Nacionalidad = "Salvadoreño",
                    FechaNacimiento = new DateTime(1935, 11, 24),
                    Activo = true
                }
            };
        }

        public IEnumerable<Autores> ObtenerTodos()
        {
            return _autores.OrderBy(a => a.Apellido);
        }

        public Autores? ObtenerPorId(int id)
        {
            return _autores.FirstOrDefault(a => a.Id == id);
        }
    }
}