using System.ComponentModel.DataAnnotations;

namespace _Net.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required, EmailAddress, MaxLength(120)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Password { get; set; } = null!; 

        [MaxLength(100)]
        public string? Nombre { get; set; }

        [Required, MaxLength(30)]
        public string Rol { get; set; } = "Empleado"; 

        public bool Activo { get; set; } = true;

        public int? IdPropietario { get; set; }

        [MaxLength(300)]
        public string? Avatar { get; set; }
    }
}
