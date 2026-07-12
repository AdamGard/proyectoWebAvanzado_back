using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Controllers
{
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
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var usuarios = await _usuarioService.ObtenerUsuarios();
            return Ok(usuarios);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerUsuariosPorId(int id)
        {
            var usuarios = await _usuarioService.ObtenerUsuariosPorId(id);
            return Ok(usuarios);
        }
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioCreateDto dto)
        {
            var usuarioCreado = await _usuarioService.CrearUsuario(dto);
            return CreatedAtAction(nameof(ObtenerUsuariosPorId), new { id = usuarioCreado.UsuarioId }, usuarioCreado);
        }
        [HttpPatch ("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] UsuarioUpdateDto dto)
        {
            var usuarioActualizado = await _usuarioService.ActualizarUsuario(id, dto);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuarioEliminado = await _usuarioService.EliminarUsuario(id);
            return Ok("Usuario eliminado con exito");
        }
    }
}
