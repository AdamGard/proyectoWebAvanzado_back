using Microsoft.AspNetCore.Mvc;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Controllers
{
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
        public async Task<IActionResult> ObtenerActividades()
        {
            var actividades = await _actividadService.ObtenerActividades();
            return Ok(actividades);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerActividadPorId(int id)
        {
            var actividad = await _actividadService.ObtenerActividadPorId(id);
            if (actividad == null) return NotFound($"Actividad con ID {id} no encontrada.");
            return Ok(actividad);
        }

        [HttpPost]
        public async Task<IActionResult> CrearActividad([FromBody] ActividadCreateDto dto)
        {
            var actividadCreada = await _actividadService.CrearActividad(dto);
            return CreatedAtAction(nameof(ObtenerActividadPorId), new { id = actividadCreada.ActividadId }, actividadCreada);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarActividad(int id, [FromBody] ActividadUpdateDto dto)
        {
            await _actividadService.ActualizarActividad(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarActividad(int id)
        {
            var eliminado = await _actividadService.EliminarActividad(id);
            if (!eliminado) return NotFound($"Actividad con ID {id} no encontrada.");
            return Ok("Actividad eliminada con éxito");
        }
    }
}
