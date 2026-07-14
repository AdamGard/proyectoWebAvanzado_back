namespace proyectoWebAvanzado.Dtos
{
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RolId { get; set; }
        public string RolNombre { get; set; } = null!; // Para mostrar el texto del rol
        public string Estado { get; set; } = null!;
        public DateTime FechaRegistro { get; set; }
    }
}
