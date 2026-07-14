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
        private readonly IConfiguration _config;
        public AuthService(AppDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Estado == "A" && u.Rol.Estado == "A");
            if (usuario == null) return null;
            bool passwordValido = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);
            if (!passwordValido) return null;

            var expiraEn = DateTime.UtcNow.AddMinutes(ObtenerMinutosExpiracion());
            var tokenString = GenerarJwtToken(usuario, expiraEn);
            return new AuthResponseDto
            {
                Token = tokenString,
                UsuarioId = usuario.UsuarioId,
                UsuarioNombre = usuario.Nombre,
                RolNombre = usuario.Rol.Nombre,
                ExpiraEn = expiraEn
            };
        }

        private string GenerarJwtToken(Data.Entities.Usuario usuario, DateTime expiraEn)
        {
            var jwtKey = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiraEn,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private double ObtenerMinutosExpiracion()
        {
            return double.TryParse(_config["Jwt:ExpireMinutes"], out var minutes) ? minutes : 120;
        }
    }
}
