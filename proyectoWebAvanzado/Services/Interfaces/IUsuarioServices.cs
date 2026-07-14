using proyectoWebAvanzado.Dtos;

namespace proyectoWebAvanzado.Services.Interfaces
{
    public interface IUsuarioServices
    {
        Task<IEnumerable<UsuarioDto>> ObtenerUsuarios();
        Task<UsuarioDto?> ObtenerUsuariosPorId(int id);
        Task<UsuarioDto> CrearUsuario(UsuarioCreateDto dto);
        Task<bool> ActualizarUsuario(int id, UsuarioUpdateDto dto);
        Task<bool> EliminarUsuario(int id);
    }
}
