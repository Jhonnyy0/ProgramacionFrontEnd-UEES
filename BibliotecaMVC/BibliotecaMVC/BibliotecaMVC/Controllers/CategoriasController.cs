using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Consulta del listado de categorías");

            var categorias = _categoriaService.ObtenerTodas();

            _logger.LogInformation("El listado de categorías devolvió {Cantidad} registro(s)", categorias.Count());

            return View(categorias);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            _logger.LogInformation("Se abrió el formulario para agregar una categoría");

            return View(new Categoria());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Categoria categoria)
        {
            _logger.LogInformation("Inicio del registro de la categoría {Nombre}", categoria.Nombre);

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            int idNuevo;

            try
            {
                idNuevo = _categoriaService.Crear(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la categoría {Nombre}", categoria.Nombre);
                ModelState.AddModelError(string.Empty, "No se pudo agregar la categoría. Intente nuevamente.");
                return View(categoria);
            }

            _logger.LogInformation("Categoría {Id} registrada correctamente: {Nombre}", idNuevo, categoria.Nombre);

            TempData["Mensaje"] = "La categoría se agregó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            _logger.LogInformation("Se abrió el formulario de edición de la categoría {Id}", id);

            var categoria = _categoriaService.ObtenerPorId(id);

            if (categoria == null)
            {
                _logger.LogWarning("No se encontró la categoría {Id} al abrir su formulario de edición", id);
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Categoria categoria)
        {
            _logger.LogInformation("Inicio de la edición de la categoría {Id}", id);

            if (id != categoria.Id)
            {
                _logger.LogWarning("El Id de la ruta ({IdRuta}) no coincide con el Id del formulario ({IdFormulario})", id, categoria.Id);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            bool actualizado;

            try
            {
                actualizado = _categoriaService.Actualizar(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los cambios de la categoría {Id}", id);
                ModelState.AddModelError(string.Empty, "No se pudieron guardar los cambios. Intente nuevamente.");
                return View(categoria);
            }

            if (!actualizado)
            {
                _logger.LogWarning("No se encontró la categoría {Id} al intentar editarla", id);
                return NotFound();
            }

            _logger.LogInformation("Categoría {Id} actualizada correctamente", id);

            TempData["Mensaje"] = "Los cambios se guardaron correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            _logger.LogInformation("Se abrió la confirmación para eliminar la categoría {Id}", id);

            var categoria = _categoriaService.ObtenerPorId(id);

            if (categoria == null)
            {
                _logger.LogWarning("No se encontró la categoría {Id} al abrir su confirmación de eliminación", id);
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            _logger.LogInformation("Inicio de la eliminación de la categoría {Id}", id);

            bool eliminado;

            try
            {
                eliminado = _categoriaService.Eliminar(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría {Id}", id);
                TempData["Mensaje"] = "No se pudo eliminar la categoría. Intente nuevamente.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            if (!eliminado)
            {
                _logger.LogWarning("No se encontró la categoría {Id} al intentar eliminarla", id);
                return NotFound();
            }

            _logger.LogInformation("Categoría {Id} eliminada correctamente", id);

            TempData["Mensaje"] = "La categoría se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}
