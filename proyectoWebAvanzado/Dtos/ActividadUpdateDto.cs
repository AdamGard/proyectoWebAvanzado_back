using System.ComponentModel.DataAnnotations;

namespace proyectoWebAvanzado.Dtos
{
    public class ActividadUpdateDto
    {
        [MaxLength(150)]
        public string? Nombre { get; set; }

        [MaxLength(50)]
        public string? Tipo { get; set; }

        [RegularExpression("^(Inicial|Basico|Intermedio|Avanzado)$", ErrorMessage = "El nivel debe ser Inicial, Basico, Intermedio o Avanzado.")]
        public string? Nivel { get; set; }

        public DateTime? Fecha { get; set; }

        // -1 se interpreta como "quitar responsable"; null significa "no tocar este campo"
        public int? ResponsableUsuarioId { get; set; }

        [MaxLength(1)]
        [RegularExpression("^[AIN]$", ErrorMessage = "El estado solo puede ser 'A', 'I' o 'N'.")]
        public string? Estado { get; set; }
    }
}
