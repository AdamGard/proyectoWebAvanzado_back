using Microsoft.EntityFrameworkCore;
using proyectoWebAvanzado.Data;
using proyectoWebAvanzado.Data.Entities;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Exceptions;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Services.Implementaciones
{
    public class ActividadService : IActividadServices
    {
        private static readonly HashSet<string> NivelesPermitidos = ["Inicial", "Basico", "Intermedio", "Avanzado"];
        private static readonly HashSet<string> EstadosPermitidos = ["A", "I", "N"];
        private readonly AppDBContext _context;

        public ActividadService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActividadDto>> ObtenerActividades()
        {
            return await _context.Actividades
                .AsNoTracking()
                .Include(a => a.Responsable)
                .OrderBy(a => a.Fecha)
                .Select(a => new ActividadDto
                {
                    ActividadId = a.ActividadId,
                    Nombre = a.Nombre,
                    Tipo = a.Tipo,
                    Nivel = a.Nivel,
                    Estado = a.Estado,
                    Fecha = a.Fecha,
                    ResponsableUsuarioId = a.ResponsableUsuarioId,
                    ResponsableNombre = a.Responsable != null ? a.Responsable.Nombre : null
                }).ToListAsync();
        }

        public async Task<ActividadDto?> ObtenerActividadPorId(int id)
        {
            if (id <= 0)
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El identificador de actividad no es válido.");
            }

            return await _context.Actividades
                .AsNoTracking()
                .Include(a => a.Responsable)
                .Where(a => a.ActividadId == id)
                .Select(a => new ActividadDto
                {
                    ActividadId = a.ActividadId,
                    Nombre = a.Nombre,
                    Tipo = a.Tipo,
                    Nivel = a.Nivel,
                    Estado = a.Estado,
                    Fecha = a.Fecha,
                    ResponsableUsuarioId = a.ResponsableUsuarioId,
                    ResponsableNombre = a.Responsable != null ? a.Responsable.Nombre : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ActividadDto> CrearActividad(ActividadCreateDto dto)
        {
            ValidarNivel(dto.Nivel);
            await ValidarResponsable(dto.ResponsableUsuarioId);

            var nuevaActividad = new Actividad
            {
                Nombre = dto.Nombre.Trim(),
                Tipo = dto.Tipo.Trim(),
                Nivel = dto.Nivel,
                Fecha = dto.Fecha.Date,
                ResponsableUsuarioId = dto.ResponsableUsuarioId,
                Estado = "A",
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };

            _context.Actividades.Add(nuevaActividad);
            await _context.SaveChangesAsync();

            return await ObtenerActividadPorId(nuevaActividad.ActividadId)
                ?? throw new ApiException(StatusCodes.Status500InternalServerError, "No se pudo recuperar la actividad creada.");
        }

        public async Task<bool> ActualizarActividad(int id, ActividadUpdateDto dto)
        {
            if (id <= 0)
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El identificador de actividad no es válido.");
            }

            var actividad = await _context.Actividades.FirstOrDefaultAsync(a => a.ActividadId == id);

            if (actividad == null)
            {
                throw new ApiException(StatusCodes.Status404NotFound, $"Actividad con ID {id} no encontrada.");
            }

            if (dto.ResponsableUsuarioId.HasValue)
            {
                if (dto.ResponsableUsuarioId.Value <= 0)
                {
                    actividad.ResponsableUsuarioId = null;
                }
                else
                {
                    await ValidarResponsable(dto.ResponsableUsuarioId.Value);
                    actividad.ResponsableUsuarioId = dto.ResponsableUsuarioId.Value;
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
            {
                actividad.Nombre = dto.Nombre.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Tipo))
            {
                actividad.Tipo = dto.Tipo.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Nivel))
            {
                ValidarNivel(dto.Nivel);
                actividad.Nivel = dto.Nivel;
            }

            if (dto.Fecha.HasValue)
            {
                actividad.Fecha = dto.Fecha.Value.Date;
            }

            if (!string.IsNullOrWhiteSpace(dto.Estado))
            {
                var estado = dto.Estado.Trim().ToUpperInvariant();
                if (!EstadosPermitidos.Contains(estado))
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, "El estado solo puede ser A, I o N.");
                }
                actividad.Estado = estado;
            }

            actividad.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarActividad(int id)
        {
            if (id <= 0)
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El identificador de actividad no es válido.");
            }

            var actividad = await _context.Actividades.FirstOrDefaultAsync(a => a.ActividadId == id);

            if (actividad == null)
            {
                return false;
            }

            _context.Actividades.Remove(actividad);
            await _context.SaveChangesAsync();

            return true;
        }

        private static void ValidarNivel(string nivel)
        {
            if (!NivelesPermitidos.Contains(nivel))
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El nivel debe ser Inicial, Basico, Intermedio o Avanzado.");
            }
        }

        private async Task ValidarResponsable(int? responsableUsuarioId)
        {
            if (!responsableUsuarioId.HasValue)
            {
                return;
            }

            var existe = await _context.Usuarios.AnyAsync(u => u.UsuarioId == responsableUsuarioId.Value && u.Estado == "A");
            if (!existe)
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El usuario responsable especificado no existe o no está activo.");
            }
        }
    }
}
