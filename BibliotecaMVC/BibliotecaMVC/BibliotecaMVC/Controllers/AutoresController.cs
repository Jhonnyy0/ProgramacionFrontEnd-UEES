using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Services;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        // GET: /Autores
        public IActionResult Index()
        {
            var autores = _autorService.ObtenerTodos();
            return View(autores);
        }

        // GET: /Autores/Detalle/1
        public IActionResult Detalle(int id)
        {
            var autor = _autorService.ObtenerPorId(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }
    }
}