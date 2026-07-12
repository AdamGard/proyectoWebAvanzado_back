using proyectoWebAvanzado.Dtos;

namespace proyectoWebAvanzado.Services.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthResponseDto?> LoignAsync(LoginDto dto);
    }
}
