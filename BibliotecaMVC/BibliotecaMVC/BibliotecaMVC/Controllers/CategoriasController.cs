using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    /// <summary>
    /// Mantenimiento de Categorías. El controlador solo coordina:
    /// recibe la petición, valida y delega el acceso a datos en el servicio.
    /// No contiene SQL ni conexiones.
    /// </summary>
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // ===================== MOSTRAR =====================
        // GET: /Categorias
        public IActionResult Index()
        {
            var categorias = _categoriaService.ObtenerTodas();
            return View(categorias);
        }

        // ===================== AGREGAR =====================
        // GET: /Categorias/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Categoria());
        }

        // POST: /Categorias/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            _categoriaService.Crear(categoria);
            TempData["Mensaje"] = "La categoría se agregó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ===================== EDITAR =====================
        // GET: /Categorias/Editar/1  -> carga los datos actuales en el formulario
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var categoria = _categoriaService.ObtenerPorId(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: /Categorias/Editar/1 -> ejecuta el UPDATE parametrizado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            bool actualizado = _categoriaService.Actualizar(categoria);

            if (!actualizado)
            {
                return NotFound();
            }

            TempData["Mensaje"] = "Los cambios se guardaron correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===================== ELIMINAR =====================
        // GET: /Categorias/Eliminar/1 -> pantalla de confirmación
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var categoria = _categoriaService.ObtenerPorId(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: /Categorias/Eliminar/1 -> ejecuta el DELETE parametrizado
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            bool eliminado = _categoriaService.Eliminar(id);

            if (!eliminado)
            {
                return NotFound();
            }

            TempData["Mensaje"] = "La categoría se eliminó correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
