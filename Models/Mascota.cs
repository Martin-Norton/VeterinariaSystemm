using System.ComponentModel.DataAnnotations;
using VeterinariaSystem.Models;

namespace VeterinariaSystem.Models
{
    public class Mascota
    {
        [Key]
        [Display(Name = "Código Mascota")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La especie es obligatoria.")]
        public string Especie { get; set; }

        public string Raza { get; set; }

        [Range(0, 100, ErrorMessage = "La edad debe ser entre 0 y 100.")]
        public int Edad { get; set; }

        [Range(0, 100, ErrorMessage = "El peso debe ser en gramos y debe ser entre 0 y 100.")]
        public int Peso { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio.")]
        public string Sexo { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un dueño.")]
        [Display(Name = "Código Dueño")]
        public int Id_Dueno { get; set; }
        public Dueno Dueno { get; set; }

        public int Estado { get; set; } = 1;
    }
}
