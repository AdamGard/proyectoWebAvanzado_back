using Microsoft.EntityFrameworkCore;
using proyectoWebAvanzado.Data;
using proyectoWebAvanzado.Data.Entities;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Services.Implementaciones
{
    public class ActividadService : IActividadServices
    {
        private readonly AppDBContext _context;
        public ActividadService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActividadDto>> ObtenerActividades()
        {
            try
            {
                return await _context.Actividades
                .Include(a => a.Responsable)
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
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las actividades", ex);
            }
        }

        public async Task<ActividadDto?> ObtenerActividadPorId(int id)
        {
            try
            {
                var actividad = await _context.Actividades
                .Include(a => a.Responsable)
                .FirstOrDefaultAsync(a => a.ActividadId == id);

                if (actividad == null) return null;

                return new ActividadDto
                {
                    ActividadId = actividad.ActividadId,
                    Nombre = actividad.Nombre,
                    Tipo = actividad.Tipo,
                    Nivel = actividad.Nivel,
                    Estado = actividad.Estado,
                    Fecha = actividad.Fecha,
                    ResponsableUsuarioId = actividad.ResponsableUsuarioId,
                    ResponsableNombre = actividad.Responsable != null ? actividad.Responsable.Nombre : null
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la actividad con ID {id}", ex);
            }
        }

        public async Task<ActividadDto> CrearActividad(ActividadCreateDto dto)
        {
            if (dto.ResponsableUsuarioId.HasValue &&
                !await _context.Usuarios.AnyAsync(u => u.UsuarioId == dto.ResponsableUsuarioId.Value))
            {
                throw new Exception("El usuario responsable especificado no existe");
            }

            var nuevaActividad = new Actividad
            {
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
                Nivel = dto.Nivel,
                Fecha = dto.Fecha.Date,
                ResponsableUsuarioId = dto.ResponsableUsuarioId,
                Estado = "A",
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };

            try
            {
                _context.Actividades.Add(nuevaActividad);
                await _context.SaveChangesAsync();
                return await ObtenerActividadPorId(nuevaActividad.ActividadId) ?? throw new Exception("Error al recuperar la actividad creada");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la actividad", ex);
            }
        }

        public async Task<bool> ActualizarActividad(int id, ActividadUpdateDto dto)
        {
            var actividad = await _context.Actividades.FindAsync(id);

            if (actividad == null)
            {
                throw new Exception($"Actividad con ID {id} no encontrada.");
            }

            // Responsable: null = no tocar, -1 = quitar responsable, cualquier otro id = validar y asignar
            if (dto.ResponsableUsuarioId.HasValue)
            {
                if (dto.ResponsableUsuarioId.Value == -1)
                {
                    actividad.ResponsableUsuarioId = null;
                }
                else
                {
                    if (!await _context.Usuarios.AnyAsync(u => u.UsuarioId == dto.ResponsableUsuarioId.Value))
                    {
                        throw new Exception("El usuario responsable especificado no existe.");
                    }
                    actividad.ResponsableUsuarioId = dto.ResponsableUsuarioId.Value;
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
            {
                actividad.Nombre = dto.Nombre;
            }

            if (!string.IsNullOrWhiteSpace(dto.Tipo))
            {
                actividad.Tipo = dto.Tipo;
            }

            if (!string.IsNullOrWhiteSpace(dto.Nivel))
            {
                actividad.Nivel = dto.Nivel;
            }

            if (dto.Fecha.HasValue)
            {
                actividad.Fecha = dto.Fecha.Value.Date;
            }

            if (!string.IsNullOrWhiteSpace(dto.Estado))
            {
                actividad.Estado = dto.Estado.ToUpper();
            }

            actividad.FechaActualizacion = DateTime.Now;

            try
            {
                _context.Actividades.Update(actividad);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar la actividad con ID {id}", ex);
            }
        }

        public async Task<bool> EliminarActividad(int id)
        {
            try
            {
                var actividad = await _context.Actividades.FindAsync(id);

                if (actividad == null) return false;

                _context.Actividades.Remove(actividad);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la actividad con ID {id}", ex);
            }
        }
    }
}
