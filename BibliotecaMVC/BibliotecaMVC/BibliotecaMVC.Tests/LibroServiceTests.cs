using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using BibliotecaMVC.Tests.Helpers;
using Xunit;

namespace BibliotecaMVC.Tests
{
    public class LibroServiceTests
    {
        [Fact]
        public void ObtenerTodos_SinLibros_DevuelveColeccionVacia()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var resultado = servicio.ObtenerTodos(null);

            Assert.Empty(resultado);
        }

        [Fact]
        public void ObtenerTodos_ConLibros_LosDevuelveOrdenadosPorTitulo()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var titulos = servicio.ObtenerTodos(null).Select(l => l.Titulo).ToList();

            var esperados = new List<string> { "Cien años de soledad", "El Aleph", "Ficciones" };
            Assert.Equal(esperados, titulos);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ObtenerTodos_BusquedaVaciaONula_DevuelveTodosLosLibros(string? buscar)
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var resultado = servicio.ObtenerTodos(buscar);

            Assert.Equal(3, resultado.Count());
        }

        [Theory]
        [InlineData("Borges", 2)]
        [InlineData("Cien", 1)]
        [InlineData("Aleph", 1)]
        [InlineData("Gabriel", 1)]
        [InlineData("Inexistente", 0)]
        public void ObtenerTodos_ConTextoDeBusqueda_DevuelveSoloLasCoincidencias(string buscar, int cantidadEsperada)
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var resultado = servicio.ObtenerTodos(buscar);

            Assert.Equal(cantidadEsperada, resultado.Count());
        }

        [Fact]
        public void ObtenerTodos_BuscandoPorAutor_DevuelveSusLibros()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var titulos = servicio.ObtenerTodos("Borges").Select(l => l.Titulo).ToList();

            var esperados = new List<string> { "El Aleph", "Ficciones" };
            Assert.Equal(esperados, titulos);
        }

        [Fact]
        public void ObtenerPorId_LibroExistente_DevuelveElLibro()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            int id = DatosDePrueba.ObtenerId(opciones, "El Aleph");
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var libro = servicio.ObtenerPorId(id);

            Assert.NotNull(libro);
            Assert.Equal("El Aleph", libro.Titulo);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        public void ObtenerPorId_LibroInexistente_DevuelveNull(int id)
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            var libro = servicio.ObtenerPorId(id);

            Assert.Null(libro);
        }

        [Fact]
        public void Crear_LibroValido_LoGuardaYAsignaLaFechaDeRegistro()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);
            var libro = DatosDePrueba.CrearLibro();
            var antes = DateTime.Now;

            int idNuevo = servicio.Crear(libro);

            using var contextoVerificacion = new BibliotecaContext(opciones);
            var guardado = contextoVerificacion.Libros.Find(idNuevo);
            Assert.True(idNuevo > 0);
            Assert.NotNull(guardado);
            Assert.Equal("Ficciones", guardado.Titulo);
            Assert.True(guardado.FechaRegistro >= antes);
            Assert.Equal(1, contextoVerificacion.Libros.Count());
        }

        [Fact]
        public void Actualizar_LibroExistente_CambiaLosCamposYDevuelveTrue()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            int id = DatosDePrueba.ObtenerId(opciones, "Ficciones");
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);
            var cambios = new Libro
            {
                Id = id,
                Titulo = "Ficciones (edición revisada)",
                Autor = "J. L. Borges",
                Editorial = "Alianza",
                Isbn = "978-0000000000",
                AnioPublicacion = 1956,
                Ejemplares = 7,
                Disponible = true
            };

            bool resultado = servicio.Actualizar(cambios);

            using var contextoVerificacion = new BibliotecaContext(opciones);
            var guardado = contextoVerificacion.Libros.Find(id);
            Assert.True(resultado);
            Assert.NotNull(guardado);
            Assert.Equal("Ficciones (edición revisada)", guardado.Titulo);
            Assert.Equal("J. L. Borges", guardado.Autor);
            Assert.Equal("Alianza", guardado.Editorial);
            Assert.Equal("978-0000000000", guardado.Isbn);
            Assert.Equal(1956, guardado.AnioPublicacion);
            Assert.Equal(7, guardado.Ejemplares);
        }

        [Fact]
        public void Actualizar_LibroExistente_ConservaLaFechaDeRegistro()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            int id = DatosDePrueba.ObtenerId(opciones, "Ficciones");
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);
            var cambios = DatosDePrueba.CrearLibro("Ficciones", "Jorge Luis Borges");
            cambios.Id = id;
            cambios.FechaRegistro = DateTime.Now;

            servicio.Actualizar(cambios);

            using var contextoVerificacion = new BibliotecaContext(opciones);
            var guardado = contextoVerificacion.Libros.Find(id);
            Assert.NotNull(guardado);
            Assert.Equal(new DateTime(2026, 1, 15), guardado.FechaRegistro);
        }

        [Fact]
        public void Actualizar_LibroInexistente_DevuelveFalseYNoAgregaRegistros()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);
            var cambios = DatosDePrueba.CrearLibro();
            cambios.Id = 999;

            bool resultado = servicio.Actualizar(cambios);

            using var contextoVerificacion = new BibliotecaContext(opciones);
            Assert.False(resultado);
            Assert.Equal(3, contextoVerificacion.Libros.Count());
        }

        [Fact]
        public void Eliminar_LibroExistente_LoEliminaYDevuelveTrue()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            int id = DatosDePrueba.ObtenerId(opciones, "El Aleph");
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            bool resultado = servicio.Eliminar(id);

            using var contextoVerificacion = new BibliotecaContext(opciones);
            Assert.True(resultado);
            Assert.Null(contextoVerificacion.Libros.Find(id));
            Assert.Equal(2, contextoVerificacion.Libros.Count());
        }

        [Fact]
        public void Eliminar_LibroInexistente_DevuelveFalseYNoAfectaLosDemas()
        {
            var opciones = DatosDePrueba.CrearOpciones();
            DatosDePrueba.CargarLibros(opciones);
            using var contexto = new BibliotecaContext(opciones);
            var servicio = new LibroService(contexto);

            bool resultado = servicio.Eliminar(999);

            using var contextoVerificacion = new BibliotecaContext(opciones);
            Assert.False(resultado);
            Assert.Equal(3, contextoVerificacion.Libros.Count());
        }
    }
}
