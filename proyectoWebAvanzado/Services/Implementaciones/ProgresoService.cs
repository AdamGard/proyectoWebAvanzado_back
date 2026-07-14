using Microsoft.EntityFrameworkCore;
using proyectoWebAvanzado.Data;
using proyectoWebAvanzado.Data.Entities;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Exceptions;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Services.Implementaciones
{
    public class ProgresoService : IProgresoServices
    {
        private readonly AppDBContext _context;

        public ProgresoService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProgresoDto>> ObtenerProgresos()
        {
            return await _context.Progresos
                .AsNoTracking()
                .Include(p => p.Usuario)
                .Include(p => p.Actividad)
                .OrderByDescending(p => p.FechaActualizacion)
                .Select(p => new ProgresoDto
                {
                    ProgresoId = p.ProgresoId,
                    UsuarioId = p.UsuarioId,
                    UsuarioNombre = p.Usuario.Nombre,
                    ActividadId = p.ActividadId,
                    ActividadNombre = p.Actividad.Nombre,
                    AvancePorcentaje = p.AvancePorcentaje,
                    Nivel = p.Nivel,
                    Estado = p.Estado
                })
                .ToListAsync();
        }

        public async Task<ProgresoResumenDto> ObtenerResumen()
        {
            var usuariosRegistrados = await _context.Usuarios.AsNoTracking().CountAsync();
            var progresos = await _context.Progresos
                .AsNoTracking()
                .Include(p => p.Usuario)
                .Include(p => p.Actividad)
                .ToListAsync();

            var categorias = progresos
                .GroupBy(p => p.Actividad.Tipo)
                .Select(g => new ProgresoCategoriaDto
                {
                    Categoria = g.Key,
                    Avance = Math.Round(g.Average(p => p.AvancePorcentaje), 2)
                })
                .OrderBy(c => c.Categoria)
                .ToList();

            var estudiantes = progresos
                .GroupBy(p => new { p.UsuarioId, p.Usuario.Nombre })
                .Select(g => new ProgresoEstudianteDto
                {
                    Nombre = g.Key.Nombre,
                    Avance = Math.Round(g.Average(p => p.AvancePorcentaje), 2),
                    Nivel = g.OrderByDescending(p => p.AvancePorcentaje).First().Nivel
                })
                .OrderByDescending(e => e.Avance)
                .ToList();

            return new ProgresoResumenDto
            {
                UsuariosRegistrados = usuariosRegistrados,
                ActividadesRealizadas = progresos.Count,
                PromedioGeneral = progresos.Count == 0 ? 0 : Math.Round(progresos.Average(p => p.AvancePorcentaje), 2),
                NivelesCompletados = progresos.Count(p => p.AvancePorcentaje >= 100),
                Categorias = categorias,
                Estudiantes = estudiantes
            };
        }

        public async Task<ProgresoDto> RegistrarOActualizar(ProgresoUpsertDto dto)
        {
            if (!await _context.Usuarios.AnyAsync(u => u.UsuarioId == dto.UsuarioId && u.Estado == "A"))
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El usuario no existe o no está activo.");
            }

            if (!await _context.Actividades.AnyAsync(a => a.ActividadId == dto.ActividadId && a.Estado == "A"))
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "La actividad no existe o no está activa.");
            }

            var progreso = await _context.Progresos
                .FirstOrDefaultAsync(p => p.UsuarioId == dto.UsuarioId && p.ActividadId == dto.ActividadId);

            if (progreso == null)
            {
                progreso = new Progreso
                {
                    UsuarioId = dto.UsuarioId,
                    ActividadId = dto.ActividadId,
                    FechaRegistro = DateTime.UtcNow
                };
                _context.Progresos.Add(progreso);
            }

            progreso.AvancePorcentaje = dto.AvancePorcentaje;
            progreso.Nivel = dto.Nivel.Trim();
            progreso.Estado = "A";
            progreso.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var creado = await _context.Progresos
                .AsNoTracking()
                .Include(p => p.Usuario)
                .Include(p => p.Actividad)
                .Where(p => p.ProgresoId == progreso.ProgresoId)
                .Select(p => new ProgresoDto
                {
                    ProgresoId = p.ProgresoId,
                    UsuarioId = p.UsuarioId,
                    UsuarioNombre = p.Usuario.Nombre,
                    ActividadId = p.ActividadId,
                    ActividadNombre = p.Actividad.Nombre,
                    AvancePorcentaje = p.AvancePorcentaje,
                    Nivel = p.Nivel,
                    Estado = p.Estado
                })
                .FirstOrDefaultAsync();

            return creado ?? throw new ApiException(StatusCodes.Status500InternalServerError, "No se pudo recuperar el progreso registrado.");
        }
    }
}
