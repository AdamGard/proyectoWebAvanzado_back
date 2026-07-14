using Microsoft.EntityFrameworkCore;
using proyectoWebAvanzado.Data;
using proyectoWebAvanzado.Data.Entities;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Exceptions;
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
            return await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .OrderBy(u => u.Nombre)
                .Select(u => new UsuarioDto
                {
                    UsuarioId = u.UsuarioId,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    RolId = u.RolId,
                    RolNombre = u.Rol.Nombre,
                    Estado = u.Estado,
                    FechaRegistro = u.FechaRegistro
                }).ToListAsync();
        }

        public async Task<UsuarioDto?> ObtenerUsuariosPorId(int id)
        {
            if (id <= 0)
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El identificador de usuario no es válido.");
            }

            return await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .Where(u => u.UsuarioId == id)
                .Select(u => new UsuarioDto
                {
                    UsuarioId = u.UsuarioId,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    RolId = u.RolId,
                    RolNombre = u.Rol.Nombre,
                    Estado = u.Estado,
                    FechaRegistro = u.FechaRegistro
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UsuarioDto> CrearUsuario(UsuarioCreateDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                throw new ApiException(StatusCodes.Status409Conflict, "El correo electrónico ya está en uso.");
            }

            if (!await _context.Roles.AnyAsync(r => r.RolId == dto.RolId && r.Estado == "A"))
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El rol especificado no existe o no está activo.");
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RolId = dto.RolId,
                Estado = "A",
                FechaRegistro = DateTime.UtcNow.Date,
                FechaActualizacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return await ObtenerUsuariosPorId(nuevoUsuario.UsuarioId)
                ?? throw new ApiException(StatusCodes.Status500InternalServerError, "No se pudo recuperar el usuario creado.");
        }

        public async Task<bool> ActualizarUsuario(int id, UsuarioUpdateDto dto)
        {
            if (id <= 0)
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El identificador de usuario no es válido.");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
            {
                throw new ApiException(StatusCodes.Status404NotFound, $"Usuario con ID {id} no encontrado.");
            }

            if (dto.RolId.HasValue)
            {
                if (!await _context.Roles.AnyAsync(r => r.RolId == dto.RolId.Value && r.Estado == "A"))
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, "El rol especificado no existe o no está activo.");
                }
                usuario.RolId = dto.RolId.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var email = dto.Email.Trim().ToLowerInvariant();
                if (await _context.Usuarios.AnyAsync(u => u.Email == email && u.UsuarioId != id))
                {
                    throw new ApiException(StatusCodes.Status409Conflict, "El correo electrónico ya está en uso por otro usuario.");
                }
                usuario.Email = email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
            {
                usuario.Nombre = dto.Nombre.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            if (!string.IsNullOrWhiteSpace(dto.Estado))
            {
                usuario.Estado = dto.Estado.Trim().ToUpperInvariant();
            }

            usuario.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarUsuario(int id)
        {
            if (id <= 0)
            {
                throw new ApiException(StatusCodes.Status400BadRequest, "El identificador de usuario no es válido.");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
            {
                return false;
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
