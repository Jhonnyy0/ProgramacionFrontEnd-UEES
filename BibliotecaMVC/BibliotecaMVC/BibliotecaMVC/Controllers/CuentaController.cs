using BibliotecaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    [AllowAnonymous]
    public class CuentaController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CuentaController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await _userManager.FindByEmailAsync(modelo.UsuarioOCorreo)
                       ?? await _userManager.FindByNameAsync(modelo.UsuarioOCorreo);

            if (usuario == null || string.IsNullOrEmpty(usuario.UserName))
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(modelo);
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                usuario.UserName,
                modelo.Password,
                isPersistent: modelo.Recordarme,
                lockoutOnFailure: false);

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(modelo);
            }

            TempData["Mensaje"] = $"Bienvenido, {usuario.UserName}.";
            TempData["TipoMensaje"] = "success";

            return RedirigirDestino(returnUrl);
        }

        [HttpGet]
        public IActionResult Registro(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegistroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel modelo, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new IdentityUser
            {
                UserName = modelo.Usuario,
                Email = modelo.Email
            };

            var resultado = await _userManager.CreateAsync(usuario, modelo.Password);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }

            await _signInManager.SignInAsync(usuario, isPersistent: false);

            TempData["Mensaje"] = $"La cuenta \"{usuario.UserName}\" se creó correctamente.";
            TempData["TipoMensaje"] = "success";

            return RedirigirDestino(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();

            TempData["Mensaje"] = "Sesión cerrada correctamente.";
            TempData["TipoMensaje"] = "info";

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        private IActionResult RedirigirDestino(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
