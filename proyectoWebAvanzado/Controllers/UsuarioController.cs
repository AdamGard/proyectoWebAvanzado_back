using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioServices _usuarioService;

        public UsuarioController(IUsuarioServices usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObtenerUsuarios()
        {
            var usuarios = await _usuarioService.ObtenerUsuarios();
            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDto>> ObtenerUsuariosPorId(int id)
        {
            var usuario = await _usuarioService.ObtenerUsuariosPorId(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = $"Usuario con ID {id} no encontrado." });
            }

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> CrearUsuario([FromBody] UsuarioCreateDto dto)
        {
            var usuarioCreado = await _usuarioService.CrearUsuario(dto);
            return CreatedAtAction(nameof(ObtenerUsuariosPorId), new { id = usuarioCreado.UsuarioId }, usuarioCreado);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] UsuarioUpdateDto dto)
        {
            await _usuarioService.ActualizarUsuario(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var eliminado = await _usuarioService.EliminarUsuario(id);
            if (!eliminado)
            {
                return NotFound(new { mensaje = $"Usuario con ID {id} no encontrado." });
            }

            return NoContent();
        }
    }
}
