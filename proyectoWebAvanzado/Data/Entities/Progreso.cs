namespace proyectoWebAvanzado.Data.Entities
{
    public class Progreso
    {
        public int ProgresoId { get; set; }
        public int UsuarioId { get; set; }
        public int ActividadId { get; set; }
        public decimal AvancePorcentaje { get; set; }
        public string Nivel { get; set; } = null!;
        public string Estado { get; set; } = "A";
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public Usuario Usuario { get; set; } = null!;
        public Actividad Actividad { get; set; } = null!;
    }
}
