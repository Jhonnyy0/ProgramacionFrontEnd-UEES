using BibliotecaMVC.Data;
using BibliotecaMVC.Reportes;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = DatosIniciales.RolAdmin + "," + DatosIniciales.RolBibliotecario + "," + DatosIniciales.RolUsuario)]
    public class ReportesController : Controller
    {
        private readonly ILibroService _libroService;

        public ReportesController(ILibroService libroService)
        {
            _libroService = libroService;
        }

        public IActionResult Index()
        {
            var autores = _libroService.ObtenerAutores();

            return View(autores);
        }

        public IActionResult LibrosPdf(string? autor)
        {
            var libros = _libroService.ObtenerPorAutor(autor).ToList();

            byte[] pdf = GeneradorReportePdf.Generar(libros, ObtenerTextoFiltro(autor));

            return File(pdf, "application/pdf", "ReporteLibros.pdf");
        }

        public IActionResult LibrosExcel(string? autor)
        {
            var libros = _libroService.ObtenerPorAutor(autor).ToList();

            byte[] excel = GeneradorReporteExcel.Generar(libros, ObtenerTextoFiltro(autor));

            return File(
                excel,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ReporteLibros.xlsx");
        }

        private static string ObtenerTextoFiltro(string? autor)
        {
            return string.IsNullOrWhiteSpace(autor) ? "Todos los autores" : $"Autor: {autor}";
        }
    }
}
