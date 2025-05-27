using System.ComponentModel.DataAnnotations;

namespace VeterinariaSystem.Models
{
    public class Consulta
    {
        [Key]
        [Display(Name = "Código Consulta")]
        public int Id { get; set; }

        [Display(Name = "Código Turno")]
        public int? Id_Turno { get; set; }

        [Required(ErrorMessage = "La fecha de la consulta es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Debe indicar el motivo.")]
        public string Motivo { get; set; }

        public string Diagnostico { get; set; }

        public string Tratamiento { get; set; }

        [Display(Name = "Archivo Adjunto")]
        public string? ArchivoAdjunto { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una mascota.")]
        public int Id_Mascota { get; set; }
        public Mascota Mascota { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un veterinario.")]
        public int Id_Veterinario { get; set; }
        public Veterinario Veterinario { get; set; }
    }
}
