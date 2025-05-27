using System.ComponentModel.DataAnnotations;

namespace VeterinariaSystem.Models
{
    public class Turno
    {
        [Key]
        [Display(Name = "Código Turno")]
        public int Id { get; set; }
            
        [Display(Name = "Código Mascota")]
        public int? Id_Mascota { get; set; }
        public Mascota Mascota { get; set; }
    
        [Required(ErrorMessage = "El motivo es obligatorio.")]
        public string Motivo { get; set; }
    
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
    
        [Required(ErrorMessage = "La hora es obligatoria.")]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }
        
        public int Estado { get; set; } = 1;
    }       
}