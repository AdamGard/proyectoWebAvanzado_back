namespace proyectoWebAvanzado.Dtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = null!;
        public string RolNombre { get; set; } = null!;
        public DateTime ExpiraEn { get; set; }
    }
}
