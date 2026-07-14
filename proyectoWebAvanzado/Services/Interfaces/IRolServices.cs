using proyectoWebAvanzado.Dtos;

namespace proyectoWebAvanzado.Services.Interfaces
{
    public interface IRolServices
    {
        Task<IEnumerable<RolDto>> ObtenerRoles();
    }
}
