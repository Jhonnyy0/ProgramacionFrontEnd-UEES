using BibliotecaMVC.Data;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class LibroService : ILibroService
    {
        private readonly BibliotecaContext _contexto;

        public LibroService(BibliotecaContext contexto)
        {
            _contexto = contexto;
        }

        public IEnumerable<Libro> ObtenerTodos(string? buscar)
        {
            var consulta = _contexto.Libros.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                consulta = consulta.Where(l =>
                    l.Titulo.Contains(buscar) ||
                    l.Autor.Contains(buscar));
            }

            return consulta
                .OrderBy(l => l.Titulo)
                .ToList();
        }

        public IEnumerable<Libro> ObtenerPorAutor(string? autor)
        {
            var consulta = _contexto.Libros.AsQueryable();

            if (!string.IsNullOrWhiteSpace(autor))
            {
                consulta = consulta.Where(l => l.Autor == autor);
            }

            return consulta
                .OrderBy(l => l.Titulo)
                .ToList();
        }

        public IEnumerable<string> ObtenerAutores()
        {
            return _contexto.Libros
                .Select(l => l.Autor)
                .Distinct()
                .OrderBy(a => a)
                .ToList();
        }

        public Libro? ObtenerPorId(int id)
        {
            return _contexto.Libros.Find(id);
        }

        public int Crear(Libro libro)
        {
            libro.FechaRegistro = DateTime.Now;

            _contexto.Libros.Add(libro);
            _contexto.SaveChanges();

            return libro.Id;
        }

        public bool Actualizar(Libro libro)
        {
            var libroExistente = _contexto.Libros.Find(libro.Id);

            if (libroExistente == null)
            {
                return false;
            }

            libroExistente.Titulo = libro.Titulo;
            libroExistente.Autor = libro.Autor;
            libroExistente.Editorial = libro.Editorial;
            libroExistente.Isbn = libro.Isbn;
            libroExistente.AnioPublicacion = libro.AnioPublicacion;
            libroExistente.Ejemplares = libro.Ejemplares;
            libroExistente.Disponible = libro.Disponible;

            _contexto.Libros.Update(libroExistente);
            _contexto.SaveChanges();

            return true;
        }

        public bool Eliminar(int id)
        {
            var libro = _contexto.Libros.Find(id);

            if (libro == null)
            {
                return false;
            }

            _contexto.Libros.Remove(libro);
            _contexto.SaveChanges();

            return true;
        }
    }
}
