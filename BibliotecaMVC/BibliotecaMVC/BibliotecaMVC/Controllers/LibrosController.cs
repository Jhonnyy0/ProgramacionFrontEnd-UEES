using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = DatosIniciales.RolAdmin + "," + DatosIniciales.RolBibliotecario + "," + DatosIniciales.RolUsuario)]
    public class LibrosController : Controller
    {
        private readonly ILibroService _libroService;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(ILibroService libroService, ILogger<LibrosController> logger)
        {
            _libroService = libroService;
            _logger = logger;
        }

        public IActionResult Index(string? buscar)
        {
            _logger.LogInformation("Consulta del listado de libros. Texto de búsqueda: {Buscar}", buscar);

            var libros = _libroService.ObtenerTodos(buscar).ToList();

            _logger.LogInformation("El listado de libros devolvió {Cantidad} registro(s)", libros.Count);

            ViewData["Buscar"] = buscar;

            return View(libros);
        }

        public IActionResult Detalle(int id)
        {
            _logger.LogInformation("Consulta del detalle del libro {Id}", id);

            var libro = _libroService.ObtenerPorId(id);

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

            int idNuevo;

            try
            {
                idNuevo = _libroService.Crear(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el libro {Titulo}", libro.Titulo);
                ModelState.AddModelError(string.Empty, "No se pudo registrar el libro. Intente nuevamente.");
                return View(libro);
            }

            _logger.LogInformation("Libro {Id} registrado correctamente: {Titulo}", idNuevo, libro.Titulo);

            TempData["Mensaje"] = $"El libro \"{libro.Titulo}\" se registró correctamente.";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = DatosIniciales.RolesGestion)]
        public IActionResult Editar(int id)
        {
            _logger.LogInformation("Se abrió el formulario de edición del libro {Id}", id);

            var libro = _libroService.ObtenerPorId(id);

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

            bool actualizado;

            try
            {
                actualizado = _libroService.Actualizar(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los cambios del libro {Id}", id);
                ModelState.AddModelError(string.Empty, "No se pudieron guardar los cambios. Intente nuevamente.");
                return View(libro);
            }

            if (!actualizado)
            {
                _logger.LogWarning("No se encontró el libro {Id} al intentar editarlo", id);
                return NotFound();
            }

            _logger.LogInformation("Libro {Id} actualizado correctamente", id);

            TempData["Mensaje"] = $"Los cambios del libro \"{libro.Titulo}\" se guardaron correctamente.";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = DatosIniciales.RolesGestion)]
        public IActionResult Eliminar(int id)
        {
            _logger.LogInformation("Se abrió la confirmación para eliminar el libro {Id}", id);

            var libro = _libroService.ObtenerPorId(id);

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

            var libro = _libroService.ObtenerPorId(id);

            if (libro == null)
            {
                _logger.LogWarning("No se encontró el libro {Id} al intentar eliminarlo", id);
                return NotFound();
            }

            string titulo = libro.Titulo;
            bool eliminado;

            try
            {
                eliminado = _libroService.Eliminar(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el libro {Id}", id);
                TempData["Mensaje"] = "No se pudo eliminar el libro. Intente nuevamente.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            if (!eliminado)
            {
                _logger.LogWarning("No se encontró el libro {Id} al intentar eliminarlo", id);
                return NotFound();
            }

            _logger.LogInformation("Libro {Id} eliminado correctamente: {Titulo}", id, titulo);

            TempData["Mensaje"] = $"El libro \"{titulo}\" se eliminó correctamente.";
            TempData["TipoMensaje"] = "warning";

            return RedirectToAction(nameof(Index));
        }
    }
}
