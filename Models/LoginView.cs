using System.ComponentModel.DataAnnotations;


namespace VeterinariaSystem.Models
{
    public class LoginView
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}
