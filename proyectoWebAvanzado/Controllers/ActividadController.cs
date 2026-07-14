using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadController : ControllerBase
    {
        private readonly IActividadServices _actividadService;

        public ActividadController(IActividadServices actividadService)
        {
            _actividadService = actividadService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActividadDto>>> ObtenerActividades()
        {
            var actividades = await _actividadService.ObtenerActividades();
            return Ok(actividades);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActividadDto>> ObtenerActividadPorId(int id)
        {
            var actividad = await _actividadService.ObtenerActividadPorId(id);
            if (actividad == null)
            {
                return NotFound(new { mensaje = $"Actividad con ID {id} no encontrada." });
            }

            return Ok(actividad);
        }

        [Authorize(Roles = "Admin,Docente")]
        [HttpPost]
        public async Task<ActionResult<ActividadDto>> CrearActividad([FromBody] ActividadCreateDto dto)
        {
            var actividadCreada = await _actividadService.CrearActividad(dto);
            return CreatedAtAction(nameof(ObtenerActividadPorId), new { id = actividadCreada.ActividadId }, actividadCreada);
        }

        [Authorize(Roles = "Admin,Docente")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ActualizarActividad(int id, [FromBody] ActividadUpdateDto dto)
        {
            await _actividadService.ActualizarActividad(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin,Docente")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarActividad(int id)
        {
            var eliminado = await _actividadService.EliminarActividad(id);
            if (!eliminado)
            {
                return NotFound(new { mensaje = $"Actividad con ID {id} no encontrada." });
            }

            return NoContent();
        }
    }
}
