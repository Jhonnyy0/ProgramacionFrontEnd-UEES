using BibliotecaMVC.Models;
using Xunit;

namespace BibliotecaMVC.Tests
{
    public class EntornoPruebasTests
    {
        [Fact]
        public void Entorno_SumaDeDosNumeros_DevuelveCuatro()
        {
            int resultado = 2 + 2;

            Assert.Equal(4, resultado);
        }

        [Fact]
        public void ProyectoReferenciado_LibroNuevo_TieneValoresPredeterminados()
        {
            var libro = new Libro();

            Assert.NotNull(libro);
            Assert.Equal(1, libro.Ejemplares);
            Assert.True(libro.Disponible);
        }
    }
}
