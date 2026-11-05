using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Tests.Helpers
{
    public static class DatosDePrueba
    {
        public static DbContextOptions<BibliotecaContext> CrearOpciones()
        {
            return new DbContextOptionsBuilder<BibliotecaContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        public static Libro CrearLibro(string titulo = "Ficciones", string autor = "Jorge Luis Borges")
        {
            return new Libro
            {
                Titulo = titulo,
                Autor = autor,
                Editorial = "Emecé",
                Isbn = "978-8420633114",
                AnioPublicacion = 1944,
                Ejemplares = 2,
                Disponible = true,
                FechaRegistro = new DateTime(2026, 1, 15)
            };
        }

        public static void CargarLibros(DbContextOptions<BibliotecaContext> opciones)
        {
            using var contexto = new BibliotecaContext(opciones);

            contexto.Libros.AddRange(
                CrearLibro("Cien años de soledad", "Gabriel García Márquez"),
                CrearLibro("Ficciones", "Jorge Luis Borges"),
                CrearLibro("El Aleph", "Jorge Luis Borges"));

            contexto.SaveChanges();
        }

        public static int ObtenerId(DbContextOptions<BibliotecaContext> opciones, string titulo)
        {
            using var contexto = new BibliotecaContext(opciones);

            return contexto.Libros.First(l => l.Titulo == titulo).Id;
        }
    }
}
