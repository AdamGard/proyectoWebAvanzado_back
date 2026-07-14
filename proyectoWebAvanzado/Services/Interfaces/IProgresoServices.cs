using proyectoWebAvanzado.Dtos;

namespace proyectoWebAvanzado.Services.Interfaces
{
    public interface IProgresoServices
    {
        Task<IEnumerable<ProgresoDto>> ObtenerProgresos();
        Task<ProgresoResumenDto> ObtenerResumen();
        Task<ProgresoDto> RegistrarOActualizar(ProgresoUpsertDto dto);
    }
}
