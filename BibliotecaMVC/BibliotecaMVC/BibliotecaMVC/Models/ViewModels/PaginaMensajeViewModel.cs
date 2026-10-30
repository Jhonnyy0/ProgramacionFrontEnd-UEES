namespace BibliotecaMVC.Models.ViewModels
{
    public class PaginaMensajeViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Sugerencia { get; set; } = string.Empty;
        public string Color { get; set; } = "danger";
        public string Icono { get; set; } = "alerta";
        public string? Referencia { get; set; }
    }
}
