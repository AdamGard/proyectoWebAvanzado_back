using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using proyectoWebAvanzado.Dtos;
using proyectoWebAvanzado.Services.Interfaces;

namespace proyectoWebAvanzado.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly IRolServices _rolServices;

        public RolController(IRolServices rolServices)
        {
            _rolServices = rolServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDto>>> ObtenerRoles()
        {
            var roles = await _rolServices.ObtenerRoles();
            return Ok(roles);
        }
    }
}
