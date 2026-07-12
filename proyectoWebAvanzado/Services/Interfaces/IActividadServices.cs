using proyectoWebAvanzado.Dtos;

namespace proyectoWebAvanzado.Services.Interfaces
{
    public interface IActividadServices
    {
        Task<IEnumerable<ActividadDto>> ObtenerActividades();
        Task<ActividadDto?> ObtenerActividadPorId(int id);
        Task<ActividadDto> CrearActividad(ActividadCreateDto dto);
        Task<bool> ActualizarActividad(int id, ActividadUpdateDto dto);
        Task<bool> EliminarActividad(int id);
    }
}
