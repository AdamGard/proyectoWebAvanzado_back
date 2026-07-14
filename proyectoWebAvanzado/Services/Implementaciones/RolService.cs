using Microsoft.EntityFrameworkCore;
using proyectoWebAvanzado.Data;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Services.Implementaciones
{
    public class RolService : IRolServices
    {
        private readonly AppDBContext _context;

        public RolService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolDto>> ObtenerRoles()
        {
            return await _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Nombre)
                .Select(r => new RolDto
                {
                    RolId = r.RolId,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Estado = r.Estado
                })
                .ToListAsync();
        }
    }
}
