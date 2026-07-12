using System.ComponentModel.DataAnnotations;

namespace proyectoWebAvanzado.Dtos
{
    public class UsuarioCreateDto
    {
        [Required, MaxLength(120)]
        public string Nombre { get; set; } = null!;

        [Required, EmailAddress, MaxLength(120)]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!; 

        [Required]
        public int RolId { get; set; }
    }
}
