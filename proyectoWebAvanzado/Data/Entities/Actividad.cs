namespace proyectoWebAvanzado.Data.Entities
{
    public class Actividad
    {
        public int ActividadId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Nivel { get; set; } = null!;
        public string Estado { get; set; } = "A";
        public DateTime Fecha { get; set; }
        public int? ResponsableUsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public Usuario? Responsable { get; set; }
    }
}
