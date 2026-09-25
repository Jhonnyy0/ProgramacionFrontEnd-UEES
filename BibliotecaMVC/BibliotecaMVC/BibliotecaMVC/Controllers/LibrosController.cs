using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _contexto;

        public LibrosController(BibliotecaContext contexto)
        {
            _contexto = contexto;
        }

        public IActionResult Index(string? buscar)
        {
            var consulta = _contexto.Libros.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                consulta = consulta.Where(l =>
                    l.Titulo.Contains(buscar) ||
                    l.Autor.Contains(buscar));
            }

            var libros = consulta
                .OrderBy(l => l.Titulo)
                .ToList();

            ViewData["Buscar"] = buscar;

            return View(libros);
        }

        public IActionResult Detalle(int id)
        {
            var libro = _contexto.Libros.Find(id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
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
        public IActionResult Crear(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            libro.FechaRegistro = DateTime.Now;

            _contexto.Libros.Add(libro);
            _contexto.SaveChanges();

            TempData["Mensaje"] = $"El libro \"{libro.Titulo}\" se registró correctamente.";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var libro = _contexto.Libros.Find(id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Libro libro)
        {
            if (id != libro.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            var libroExistente = _contexto.Libros.Find(id);

            if (libroExistente == null)
            {
                return NotFound();
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

            TempData["Mensaje"] = $"Los cambios del libro \"{libroExistente.Titulo}\" se guardaron correctamente.";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var libro = _contexto.Libros.Find(id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            var libro = _contexto.Libros.Find(id);

            if (libro == null)
            {
                return NotFound();
            }

            string titulo = libro.Titulo;

            _contexto.Libros.Remove(libro);
            _contexto.SaveChanges();

            TempData["Mensaje"] = $"El libro \"{titulo}\" se eliminó correctamente.";
            TempData["TipoMensaje"] = "warning";

            return RedirectToAction(nameof(Index));
        }
    }
}
