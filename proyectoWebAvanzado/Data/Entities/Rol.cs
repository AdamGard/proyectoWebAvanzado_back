namespace proyectoWebAvanzado.Data.Entities
{
    public class Rol
    {
        public int RolId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = "A";
        public DateTime FechaCreacion { get; set; }
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
