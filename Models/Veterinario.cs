using System.ComponentModel.DataAnnotations;

namespace VeterinariaSystem.Models
{
    public class Veterinario
    {
        [Key]
        [Display(Name = "Código Veterinario")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El ¨DNI es obligatorio.")]
        public int DNI { get; set; }

        [Required(ErrorMessage = "La matrícula es obligatoria.")]
        public string Matricula { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un email válido.")]
        public string Email { get; set; }

        [Display(Name = "Estado")]
        public int Estado { get; set; } = 1;
    }
}
