using Microsoft.EntityFrameworkCore;
using proyectoWebAvanzado.Data;
using proyectoWebAvanzado.Data.Entities;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Services.Implementaciones
{
    public class UsuarioService : IUsuarioServices
    {
        private readonly AppDBContext _context;
        public UsuarioService(AppDBContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<UsuarioDto>> ObtenerUsuarios()
        {
            try
            {
                return await _context.Usuarios
                .Include(u => u.Rol)
                .Select(u => new UsuarioDto
                {
                    UsuarioId = u.UsuarioId,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    RolId = u.RolId,
                    RolNombre = u.Rol.Nombre,
                    Estado = u.Estado
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios", ex);
            }
        }

        public async Task<UsuarioDto?> ObtenerUsuariosPorId(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);
                return new UsuarioDto
                {
                    UsuarioId = usuario.UsuarioId,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    RolId = usuario.RolId,
                    RolNombre = usuario.Rol.Nombre,
                    Estado = usuario.Estado
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el usuario con ID {id}", ex);
            }
        }

        public async Task<UsuarioDto> CrearUsuario(UsuarioCreateDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            {
                throw new Exception("El correo electrónico ya está en uso");
            }
            if (!await _context.Roles.AnyAsync(r => r.RolId == dto.RolId))
            {
                throw new Exception("El rol especificado no existe");
            }
            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RolId = dto.RolId,
                Estado = "A",
                FechaRegistro = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };

            try
            {
                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();
                return await ObtenerUsuariosPorId(nuevoUsuario.UsuarioId) ?? throw new Exception("Error al recuperar el usuario creado");
            }
            
            catch (Exception ex)
            {
                throw new Exception("Error al crear el usuario", ex);
            }
        }

        public async Task<bool> ActualizarUsuario(int id, UsuarioUpdateDto dto)
        {
            // Con el Global Query Filter activo, FindAsync solo encontrará usuarios con estado 'A'.
            // Si necesitas poder editar usuarios inactivos, usa:
            // var usuario = await _context.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.UsuarioId == id);
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                throw new Exception($"Usuario con ID {id} no encontrado o se encuentra inactivo.");
            }

            // 1. Validar el Rol SOLO si el frontend decidió enviarlo
            if (dto.RolId.HasValue)
            {
                if (!await _context.Roles.AnyAsync(r => r.RolId == dto.RolId.Value))
                {
                    throw new Exception("El rol especificado no existe.");
                }
                usuario.RolId = dto.RolId.Value;
            }

            // 2. Validar el Email SOLO si el frontend decidió enviarlo
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.UsuarioId != id))
                {
                    throw new Exception("El correo electrónico ya está en uso por otro usuario.");
                }
                usuario.Email = dto.Email;
            }

            // 3. Actualizar el resto de campos si vienen informados
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
            {
                usuario.Nombre = dto.Nombre;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuario.PasswordHash = dto.Password;
            }

            if (!string.IsNullOrWhiteSpace(dto.Estado))
            {
                usuario.Estado = dto.Estado.ToUpper();
            }

            usuario.FechaActualizacion = DateTime.Now;
            try
            {
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el usuario con ID {id}", ex);
            }
        }

        public async Task<bool> EliminarUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null) return false;

                
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el usuario con ID {id}", ex);
            }

        }
    }
}
