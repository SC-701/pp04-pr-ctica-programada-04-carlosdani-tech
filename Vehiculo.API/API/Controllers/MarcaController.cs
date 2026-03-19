using Abstracciones.Interfaces.DA;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcaController : ControllerBase
    {
        private readonly ICatalogoDA _catalogoDA;

        public MarcaController(ICatalogoDA catalogoDA)
        {
            _catalogoDA = catalogoDA;
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var marcas = await _catalogoDA.ObtenerMarcas();
            return Ok(marcas);
        }
    }
}
