using System.ComponentModel.DataAnnotations;

namespace proyectoWebAvanzado.Dtos
{
    public class UsuarioUpdateDto
    {
        [MaxLength(120)]
        public string? Nombre { get; set; }

        [EmailAddress, MaxLength(120)]
        public string? Email { get; set; }

        [MinLength(5)]
        public string? Password { get; set; }

        // Al ser int?, si el front no lo envía, será null y no se actualizará
        public int? RolId { get; set; }

        [MaxLength(1)]
        [RegularExpression("^[AIN]$", ErrorMessage = "El estado solo puede ser 'A', 'I' o 'N'.")]
        public string? Estado { get; set; }
    }
}
