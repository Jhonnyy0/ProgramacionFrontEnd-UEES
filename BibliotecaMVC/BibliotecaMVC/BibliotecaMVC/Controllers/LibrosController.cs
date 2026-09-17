using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _contexto;

        public LibrosController(BibliotecaContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<IActionResult> Index()
        {
            var libros = await _contexto.Libros
                .AsNoTracking()
                .OrderBy(l => l.Titulo)
                .ToListAsync();

            return View(libros);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var libro = new Libro
            {
                AnioPublicacion = DateTime.Now.Year,
                Ejemplares = 1,
                Disponible = true,
                FechaRegistro = DateTime.Now
            };

            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            libro.FechaRegistro = DateTime.Now;

            _contexto.Libros.Add(libro);
            await _contexto.SaveChangesAsync();

            TempData["Mensaje"] = $"El libro \"{libro.Titulo}\" se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var libro = await _contexto.Libros
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }
    }
}
