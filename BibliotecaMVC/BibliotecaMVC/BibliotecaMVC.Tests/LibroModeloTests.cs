using System.ComponentModel.DataAnnotations;
using BibliotecaMVC.Models;
using BibliotecaMVC.Tests.Helpers;
using Xunit;

namespace BibliotecaMVC.Tests
{
    public class LibroModeloTests
    {
        private static List<ValidationResult> Validar(Libro libro)
        {
            var resultados = new List<ValidationResult>();
            var contexto = new ValidationContext(libro);

            Validator.TryValidateObject(libro, contexto, resultados, true);

            return resultados;
        }

        private static bool TieneErrorEn(List<ValidationResult> errores, string campo)
        {
            return errores.Any(e => e.MemberNames.Contains(campo));
        }

        [Fact]
        public void LibroValido_NoTieneErrores()
        {
            var libro = DatosDePrueba.CrearLibro();

            var errores = Validar(libro);

            Assert.Empty(errores);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Titulo_VacioOConEspacios_EsInvalido(string titulo)
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Titulo = titulo;

            var errores = Validar(libro);

            Assert.True(TieneErrorEn(errores, "Titulo"));
        }

        [Fact]
        public void Titulo_ConMasDe150Caracteres_EsInvalido()
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Titulo = new string('A', 151);

            var errores = Validar(libro);

            Assert.True(TieneErrorEn(errores, "Titulo"));
        }

        [Fact]
        public void Titulo_Con150Caracteres_EsValido()
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Titulo = new string('A', 150);

            var errores = Validar(libro);

            Assert.Empty(errores);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Autor_VacioOConEspacios_EsInvalido(string autor)
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Autor = autor;

            var errores = Validar(libro);

            Assert.True(TieneErrorEn(errores, "Autor"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1449)]
        [InlineData(2101)]
        public void AnioPublicacion_FueraDeRango_EsInvalido(int anio)
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.AnioPublicacion = anio;

            var errores = Validar(libro);

            Assert.True(TieneErrorEn(errores, "AnioPublicacion"));
        }

        [Theory]
        [InlineData(1450)]
        [InlineData(2100)]
        public void AnioPublicacion_EnLosLimites_EsValido(int anio)
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.AnioPublicacion = anio;

            var errores = Validar(libro);

            Assert.Empty(errores);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(10000)]
        public void Ejemplares_FueraDeRango_EsInvalido(int ejemplares)
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Ejemplares = ejemplares;

            var errores = Validar(libro);

            Assert.True(TieneErrorEn(errores, "Ejemplares"));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(9999)]
        public void Ejemplares_EnLosLimites_EsValido(int ejemplares)
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Ejemplares = ejemplares;

            var errores = Validar(libro);

            Assert.Empty(errores);
        }

        [Fact]
        public void Isbn_ConMasDe20Caracteres_EsInvalido()
        {
            var libro = DatosDePrueba.CrearLibro();
            libro.Isbn = new string('9', 21);

            var errores = Validar(libro);

            Assert.True(TieneErrorEn(errores, "Isbn"));
        }

        [Fact]
        public void TituloCompleto_CombinaTituloYAnio()
        {
            var libro = DatosDePrueba.CrearLibro();

            Assert.Equal("Ficciones (1944)", libro.TituloCompleto);
        }
    }
}
