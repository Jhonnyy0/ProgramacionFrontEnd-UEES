using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = DatosIniciales.RolAdmin + "," + DatosIniciales.RolBibliotecario + "," + DatosIniciales.RolUsuario)]
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _contexto;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(BibliotecaContext contexto, ILogger<LibrosController> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }

        public IActionResult Index(string? buscar)
        {
            _logger.LogInformation("Consulta del listado de libros. Texto de búsqueda: {Buscar}", buscar);

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

            _logger.LogInformation("El listado de libros devolvió {Cantidad} registro(s)", libros.Count);

            ViewData["Buscar"] = buscar;

            return View(libros);
        }

        public IActionResult Detalle(int id)
        {
            _logger.LogInformation("Consulta del detalle del libro {Id}", id);

            var libro = _contexto.Libros.Find(id);

            if (libro == null)
            {
                _logger.LogWarning("No se encontró el libro {Id} al consultar su detalle", id);
                return NotFound();
            }

            return View(libro);
        }

        [HttpGet]
        [Authorize(Roles = DatosIniciales.RolesGestion)]
        public IActionResult Crear()
        {
            _logger.LogInformation("Se abrió el formulario para registrar un libro");

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
        [Authorize(Roles = DatosIniciales.RolesGestion)]
        public IActionResult Crear(Libro libro)
        {
            _logger.LogInformation("Inicio del registro de un libro por el usuario {Usuario}", User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            libro.FechaRegistro = DateTime.Now;

            try
            {
                _contexto.Libros.Add(libro);
                _contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el libro {Titulo}", libro.Titulo);
                ModelState.AddModelError(string.Empty, "No se pudo registrar el libro. Intente nuevamente.");
                return View(libro);
            }

            _logger.LogInformation("Libro {Id} registrado correctamente: {Titulo}", libro.Id, libro.Titulo);

            TempData["Mensaje"] = $"El libro \"{libro.Titulo}\" se registró correctamente.";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = DatosIniciales.RolesGestion)]
        public IActionResult Editar(int id)
        {
            _logger.LogInformation("Se abrió el formulario de edición del libro {Id}", id);

            var libro = _contexto.Libros.Find(id);

            if (libro == null)
            {
                _logger.LogWarning("No se encontró el libro {Id} al abrir su formulario de edición", id);
                return NotFound();
            }

            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = DatosIniciales.RolesGestion)]
        public IActionResult Editar(int id, Libro libro)
        {
            _logger.LogInformation("Inicio de la edición del libro {Id} por el usuario {Usuario}", id, User.Identity?.Name);

            if (id != libro.Id)
            {
                _logger.LogWarning("El Id de la ruta ({IdRuta}) no coincide con el Id del formulario ({IdFormulario})", id, libro.Id);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            var libroExistente = _contexto.Libros.Find(id);

            if (libroExistente == null)
            {
                _logger.LogWarning("No se encontró el libro {Id} al intentar editarlo", id);
                return NotFound();
            }

            libroExistente.Titulo = libro.Titulo;
            libroExistente.Autor = libro.Autor;
            libroExistente.Editorial = libro.Editorial;
            libroExistente.Isbn = libro.Isbn;
            libroExistente.AnioPublicacion = libro.AnioPublicacion;
            libroExistente.Ejemplares = libro.Ejemplares;
            libroExistente.Disponible = libro.Disponible;

            try
            {
                _contexto.Libros.Update(libroExistente);
                _contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los cambios del libro {Id}", id);
                ModelState.AddModelError(string.Empty, "No se pudieron guardar los cambios. Intente nuevamente.");
                return View(libro);
            }

            _logger.LogInformation("Libro {Id} actualizado correctamente", id);

            TempData["Mensaje"] = $"Los cambios del libro \"{libroExistente.Titulo}\" se guardaron correctamente.";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = DatosIniciales.RolesGestion)]
        public IActionResult Eliminar(int id)
        {
            _logger.LogInformation("Se abrió la confirmación para eliminar el libro {Id}", id);

            var libro = _contexto.Libros.Find(id);

            if (libro == null)
            {
                _logger.LogWarning("No se encontró el libro {Id} al abrir su confirmación de eliminación", id);
                return NotFound();
            }

            return View(libro);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = DatosIniciales.RolesGestion)]
        public IActionResult EliminarConfirmado(int id)
        {
            _logger.LogInformation("Inicio de la eliminación del libro {Id} por el usuario {Usuario}", id, User.Identity?.Name);

            var libro = _contexto.Libros.Find(id);

            if (libro == null)
            {
                _logger.LogWarning("No se encontró el libro {Id} al intentar eliminarlo", id);
                return NotFound();
            }

            string titulo = libro.Titulo;

            try
            {
                _contexto.Libros.Remove(libro);
                _contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el libro {Id}", id);
                TempData["Mensaje"] = "No se pudo eliminar el libro. Intente nuevamente.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Libro {Id} eliminado correctamente: {Titulo}", id, titulo);

            TempData["Mensaje"] = $"El libro \"{titulo}\" se eliminó correctamente.";
            TempData["TipoMensaje"] = "warning";

            return RedirectToAction(nameof(Index));
        }
    }
}
