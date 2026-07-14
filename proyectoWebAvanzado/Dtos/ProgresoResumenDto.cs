namespace proyectoWebAvanzado.Dtos
{
    public class ProgresoResumenDto
    {
        public int UsuariosRegistrados { get; set; }
        public int ActividadesRealizadas { get; set; }
        public decimal PromedioGeneral { get; set; }
        public int NivelesCompletados { get; set; }
        public IEnumerable<ProgresoCategoriaDto> Categorias { get; set; } = [];
        public IEnumerable<ProgresoEstudianteDto> Estudiantes { get; set; } = [];
    }

    public class ProgresoCategoriaDto
    {
        public string Categoria { get; set; } = null!;
        public decimal Avance { get; set; }
    }

    public class ProgresoEstudianteDto
    {
        public string Nombre { get; set; } = null!;
        public decimal Avance { get; set; }
        public string Nivel { get; set; } = null!;
    }
}
