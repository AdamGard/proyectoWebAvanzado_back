namespace proyectoWebAvanzado.Data.Entities
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int RolId { get; set; }
        public string Estado { get; set; } = "A";
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public Rol Rol { get; set; } = null!;
    }
}
