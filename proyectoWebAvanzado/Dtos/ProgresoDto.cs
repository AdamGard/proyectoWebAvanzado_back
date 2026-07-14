namespace proyectoWebAvanzado.Dtos
{
    public class ProgresoDto
    {
        public int ProgresoId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = null!;
        public int ActividadId { get; set; }
        public string ActividadNombre { get; set; } = null!;
        public decimal AvancePorcentaje { get; set; }
        public string Nivel { get; set; } = null!;
        public string Estado { get; set; } = null!;
    }
}
