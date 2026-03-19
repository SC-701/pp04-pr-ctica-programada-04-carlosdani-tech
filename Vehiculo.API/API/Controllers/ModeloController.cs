using Abstracciones.Interfaces.DA;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModeloController : ControllerBase
    {
        private readonly ICatalogoDA _catalogoDA;

        public ModeloController(ICatalogoDA catalogoDA)
        {
            _catalogoDA = catalogoDA;
        }

        [HttpGet("{idMarca:guid}")]
        public async Task<IActionResult> Obtener([FromRoute] Guid idMarca)
        {
            var modelos = await _catalogoDA.ObtenerModelos(idMarca);
            return Ok(modelos);
        }
    }
}
