using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaSystem.Models
{
    public enum enRoles
    {
        Administrador = 1,
        Veterinario = 2,
        Dueno = 3,
    }

    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Clave { get; set; }

        public string? Avatar { get; set; }

        [NotMapped]
        public IFormFile? AvatarFile { get; set; }

        [Required]
        public int Rol { get; set; }

        [NotMapped]
        public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

        public int? VeterinarioId { get; set; }

        public Veterinario? Veterinario { get; set; }

        public int? DuenoId { get; set; }
        
        public Dueno? Dueno { get; set; }

        public static IDictionary<int, string> ObtenerRoles()
        {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipoEnumRoles = typeof(enRoles);
            foreach (var valor in Enum.GetValues(tipoEnumRoles))
            {
                roles.Add((int)valor, Enum.GetName(tipoEnumRoles, valor));
            }
            return roles;
        }
    }
}
