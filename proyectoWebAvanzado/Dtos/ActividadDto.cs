namespace proyectoWebAvanzado.Dtos
{
    public class ActividadDto
    {
        public int ActividadId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Nivel { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public int? ResponsableUsuarioId { get; set; }
        public string? ResponsableNombre { get; set; } // Para mostrar el nombre del responsable
    }
}
