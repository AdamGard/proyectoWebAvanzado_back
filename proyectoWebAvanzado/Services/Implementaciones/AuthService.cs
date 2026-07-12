using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using proyectoWebAvanzado.Data;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace proyectoWebAvanzado.Services.Implementaciones
{
    public class AuthService : IAuthServices
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _config; //lectura de configuraciones del appsettings
        public AuthService(AppDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<AuthResponseDto?> LoignAsync(LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null) return null;
            bool passwordValido = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);
            if (!passwordValido) return null;

            var tokenString = GenerarJwtToken(usuario);
            return new AuthResponseDto
            {
                Token = tokenString,
                UsuarioNombre = usuario.Nombre,
                RolNombre = usuario.Rol.Nombre
            };
        }
        private string GenerarJwtToken(Data.Entities.Usuario usuario)
        {
            // Leemos la configuración del appsettings.json
            var jwtKey = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireMinutes = Convert.ToDouble(_config["Jwt:ExpireMinutes"]);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Creamos los "Claims" (la información que irá dentro del token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre) // ¡Clave para la autorización basada en roles!
            };

            // Configuramos el token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: credentials);

            // Devolvemos el token serializado como un string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
