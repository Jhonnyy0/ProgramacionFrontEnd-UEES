using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models.ViewModels
{
    /// <summary>
    /// Datos que viajan desde la vista de inicio de sesión al controlador.
    /// Es un ViewModel: no se guarda en la base, solo transporta el formulario.
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese su usuario o correo electrónico.")]
        [Display(Name = "Usuario o correo electrónico")]
        public string UsuarioOCorreo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese su contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Mantener mi sesión iniciada")]
        public bool Recordarme { get; set; }
    }
}
