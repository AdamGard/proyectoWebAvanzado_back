using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProgresoController : ControllerBase
    {
        private readonly IProgresoServices _progresoServices;

        public ProgresoController(IProgresoServices progresoServices)
        {
            _progresoServices = progresoServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgresoDto>>> ObtenerProgresos()
        {
            var progresos = await _progresoServices.ObtenerProgresos();
            return Ok(progresos);
        }

        [HttpGet("resumen")]
        public async Task<ActionResult<ProgresoResumenDto>> ObtenerResumen()
        {
            var resumen = await _progresoServices.ObtenerResumen();
            return Ok(resumen);
        }

        [Authorize(Roles = "Admin,Docente")]
        [HttpPost]
        public async Task<ActionResult<ProgresoDto>> RegistrarOActualizar([FromBody] ProgresoUpsertDto dto)
        {
            var progreso = await _progresoServices.RegistrarOActualizar(dto);
            return Ok(progreso);
        }
    }
}
