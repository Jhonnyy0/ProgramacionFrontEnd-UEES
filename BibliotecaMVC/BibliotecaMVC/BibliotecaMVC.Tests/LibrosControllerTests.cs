using BibliotecaMVC.Controllers;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using BibliotecaMVC.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace BibliotecaMVC.Tests
{
    public class LibrosControllerTests
    {
        private static LibrosController CrearControlador(Mock<ILibroService> servicio)
        {
            var contextoHttp = new DefaultHttpContext();

            return new LibrosController(servicio.Object, NullLogger<LibrosController>.Instance)
            {
                ControllerContext = new ControllerContext { HttpContext = contextoHttp },
                TempData = new TempDataDictionary(contextoHttp, Mock.Of<ITempDataProvider>())
            };
        }

        [Fact]
        public void Index_ConLibros_DevuelveLaVistaConElListado()
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerTodos(null)).Returns(new List<Libro>
            {
                DatosDePrueba.CrearLibro("Ficciones", "Jorge Luis Borges"),
                DatosDePrueba.CrearLibro("El Aleph", "Jorge Luis Borges")
            });
            var controlador = CrearControlador(servicio);

            var resultado = controlador.Index(null);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsAssignableFrom<IEnumerable<Libro>>(vista.Model);
            Assert.Equal(2, modelo.Count());
            servicio.Verify(s => s.ObtenerTodos(null), Times.Once());
        }

        [Fact]
        public void Index_SinLibros_DevuelveLaVistaConListaVacia()
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerTodos(null)).Returns(new List<Libro>());
            var controlador = CrearControlador(servicio);

            var resultado = controlador.Index(null);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsAssignableFrom<IEnumerable<Libro>>(vista.Model);
            Assert.Empty(modelo);
        }

        [Fact]
        public void Index_ConTextoDeBusqueda_LoPasaAlServicioYLoConservaEnLaVista()
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerTodos("Borges")).Returns(new List<Libro>
            {
                DatosDePrueba.CrearLibro("Ficciones", "Jorge Luis Borges")
            });
            var controlador = CrearControlador(servicio);

            var resultado = controlador.Index("Borges");

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Equal("Borges", vista.ViewData["Buscar"] as string);
            servicio.Verify(s => s.ObtenerTodos("Borges"), Times.Once());
        }

        [Fact]
        public void Detalle_LibroExistente_DevuelveLaVistaConElLibro()
        {
            var libro = DatosDePrueba.CrearLibro("El Aleph", "Jorge Luis Borges");
            libro.Id = 3;
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerPorId(3)).Returns(libro);
            var controlador = CrearControlador(servicio);

            var resultado = controlador.Detalle(3);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsType<Libro>(vista.Model);
            Assert.Equal("El Aleph", modelo.Titulo);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        public void Detalle_LibroInexistente_DevuelveNotFound(int id)
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerPorId(id)).Returns((Libro?)null);
            var controlador = CrearControlador(servicio);

            var resultado = controlador.Detalle(id);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public void Crear_ModeloInvalido_NoGuardaYDevuelveLaVista()
        {
            var servicio = new Mock<ILibroService>();
            var controlador = CrearControlador(servicio);
            controlador.ModelState.AddModelError("Titulo", "El título es obligatorio.");
            var libro = new Libro();

            var resultado = controlador.Crear(libro);

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Same(libro, vista.Model);
            servicio.Verify(s => s.Crear(It.IsAny<Libro>()), Times.Never());
        }

        [Fact]
        public void Crear_ModeloValido_GuardaYRedirigeAlListado()
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.Crear(It.IsAny<Libro>())).Returns(10);
            var controlador = CrearControlador(servicio);

            var resultado = controlador.Crear(DatosDePrueba.CrearLibro());

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            Assert.Equal("success", controlador.TempData["TipoMensaje"] as string);
            servicio.Verify(s => s.Crear(It.IsAny<Libro>()), Times.Once());
        }

        [Fact]
        public void Crear_ErrorDelServicio_MuestraElErrorYNoRedirige()
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.Crear(It.IsAny<Libro>()))
                .Throws(new InvalidOperationException("Fallo simulado"));
            var controlador = CrearControlador(servicio);

            var resultado = controlador.Crear(DatosDePrueba.CrearLibro());

            Assert.IsType<ViewResult>(resultado);
            Assert.False(controlador.ModelState.IsValid);
        }

        [Fact]
        public void Editar_Get_LibroExistente_DevuelveLaVistaConElLibro()
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Id = 4;
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerPorId(4)).Returns(libro);
            var controlador = CrearControlador(servicio);

            var resultado = controlador.Editar(4);

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Same(libro, vista.Model);
        }

        [Fact]
        public void Editar_Get_LibroInexistente_DevuelveNotFound()
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerPorId(99)).Returns((Libro?)null);
            var controlador = CrearControlador(servicio);

            var resultado = controlador.Editar(99);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public void Editar_IdDistintoAlDelFormulario_DevuelveBadRequestYNoActualiza()
        {
            var servicio = new Mock<ILibroService>();
            var controlador = CrearControlador(servicio);
            var libro = DatosDePrueba.CrearLibro();
            libro.Id = 5;

            var resultado = controlador.Editar(3, libro);

            Assert.IsType<BadRequestResult>(resultado);
            servicio.Verify(s => s.Actualizar(It.IsAny<Libro>()), Times.Never());
        }

        [Fact]
        public void Editar_LibroInexistente_DevuelveNotFound()
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.Actualizar(It.IsAny<Libro>())).Returns(false);
            var controlador = CrearControlador(servicio);
            var libro = DatosDePrueba.CrearLibro();
            libro.Id = 8;

            var resultado = controlador.Editar(8, libro);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public void Editar_LibroExistente_ActualizaYRedirigeAlListado()
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.Actualizar(It.IsAny<Libro>())).Returns(true);
            var controlador = CrearControlador(servicio);
            var libro = DatosDePrueba.CrearLibro();
            libro.Id = 4;

            var resultado = controlador.Editar(4, libro);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            Assert.Equal("success", controlador.TempData["TipoMensaje"] as string);
            servicio.Verify(s => s.Actualizar(It.Is<Libro>(l => l.Id == 4)), Times.Once());
        }

        [Fact]
        public void Eliminar_LibroInexistente_DevuelveNotFoundYNoElimina()
        {
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerPorId(9)).Returns((Libro?)null);
            var controlador = CrearControlador(servicio);

            var resultado = controlador.EliminarConfirmado(9);

            Assert.IsType<NotFoundResult>(resultado);
            servicio.Verify(s => s.Eliminar(It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void Eliminar_LibroExistente_EliminaYRedirigeAlListado()
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Id = 2;
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerPorId(2)).Returns(libro);
            servicio.Setup(s => s.Eliminar(2)).Returns(true);
            var controlador = CrearControlador(servicio);

            var resultado = controlador.EliminarConfirmado(2);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            Assert.Equal("warning", controlador.TempData["TipoMensaje"] as string);
            servicio.Verify(s => s.Eliminar(2), Times.Once());
        }

        [Fact]
        public void Eliminar_ErrorDelServicio_RedirigeConMensajeDeError()
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Id = 2;
            var servicio = new Mock<ILibroService>();
            servicio.Setup(s => s.ObtenerPorId(2)).Returns(libro);
            servicio.Setup(s => s.Eliminar(2)).Throws(new InvalidOperationException("Fallo simulado"));
            var controlador = CrearControlador(servicio);

            var resultado = controlador.EliminarConfirmado(2);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            Assert.Equal("danger", controlador.TempData["TipoMensaje"] as string);
        }
    }
}
