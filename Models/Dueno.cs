using System.ComponentModel.DataAnnotations;

namespace VeterinariaSystem.Models
{
    public class Dueno
    {
        [Key]
        [Display(Name = "Código Dueño")]
        public int Id { get; set; }

        [Display(Name = "DNI")]
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        public int DNI { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; }

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Debe ingresar un email válido.")]
        public string Email { get; set; }

        public int Estado { get; set; } = 1;
    }
}
