using System.ComponentModel.DataAnnotations;

namespace proyectoWebAvanzado.Dtos
{
    public class ProgresoUpsertDto
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ActividadId { get; set; }

        [Range(0, 100)]
        public decimal AvancePorcentaje { get; set; }

        [Required, MaxLength(30)]
        public string Nivel { get; set; } = null!;
    }
}
