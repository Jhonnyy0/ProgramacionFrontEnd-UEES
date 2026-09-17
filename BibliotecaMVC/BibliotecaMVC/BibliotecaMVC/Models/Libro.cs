using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede exceder los 150 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El autor no puede exceder los 100 caracteres.")]
        [Display(Name = "Autor")]
        public string Autor { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La editorial no puede exceder los 100 caracteres.")]
        [Display(Name = "Editorial")]
        public string? Editorial { get; set; }

        [StringLength(20, ErrorMessage = "El ISBN no puede exceder los 20 caracteres.")]
        [Display(Name = "ISBN")]
        public string? Isbn { get; set; }

        [Range(1450, 2100, ErrorMessage = "Ingrese un año de publicación válido.")]
        [Display(Name = "Año de publicación")]
        public int AnioPublicacion { get; set; }

        [Range(1, 9999, ErrorMessage = "La cantidad de ejemplares debe ser mayor a cero.")]
        [Display(Name = "Ejemplares")]
        public int Ejemplares { get; set; } = 1;

        [Display(Name = "Disponible")]
        public bool Disponible { get; set; } = true;

        [Display(Name = "Fecha de registro")]
        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [NotMapped]
        public string TituloCompleto => $"{Titulo} ({AnioPublicacion})";
    }
}
