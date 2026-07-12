using System.ComponentModel.DataAnnotations;

namespace proyectoWebAvanzado.Dtos
{
    public class ActividadCreateDto
    {
        [Required, MaxLength(150)]
        public string Nombre { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Tipo { get; set; } = null!;

        [Required]
        [RegularExpression("^(Inicial|Basico|Intermedio|Avanzado)$", ErrorMessage = "El nivel debe ser Inicial, Basico, Intermedio o Avanzado.")]
        public string Nivel { get; set; } = null!;

        [Required]
        public DateTime Fecha { get; set; }

        // Opcional: si no se envía, la actividad queda sin responsable asignado
        public int? ResponsableUsuarioId { get; set; }
    }
}
