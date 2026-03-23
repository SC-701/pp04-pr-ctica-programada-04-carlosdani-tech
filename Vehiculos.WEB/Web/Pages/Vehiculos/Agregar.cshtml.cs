using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    public class AgregarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public AgregarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        [BindProperty]
        public VehiculoRequest vehiculo { get; set; } = new();

        [BindProperty]
        public List<SelectListItem> marcas { get; set; } = new();

        [BindProperty]
        public List<SelectListItem> modelos { get; set; } = new();

        [BindProperty]
        public Guid marcaseleccionada { get; set; }

        public async Task<IActionResult> OnGet()
        {
            await CargarFormulario();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await CargarFormulario();
                return Page();
            }

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "AgregarVehiculo");
            using var cliente = new HttpClient();
            var respuesta = await cliente.PostAsJsonAsync(endpoint, vehiculo);
            respuesta.EnsureSuccessStatusCode();
            return RedirectToPage("./Index");
        }

        private async Task CargarFormulario()
        {
            await ObtenerMarcas();

            modelos = new List<SelectListItem>();
            if (marcaseleccionada != Guid.Empty)
            {
                modelos = (await ObtenerModelos(marcaseleccionada)).Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Nombre,
                    Selected = m.Id == vehiculo.IdModelo
                }).ToList();
            }
        }

        private async Task ObtenerMarcas()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerMarcas");

            Console.WriteLine($"URL completa: {endpoint}");
            System.Diagnostics.Debug.WriteLine($"URL completa: {endpoint}");

            using var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();
            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resultadoDeserializado = JsonSerializer.Deserialize<List<Marca>>(resultado, opciones) ?? new List<Marca>();

            marcas = resultadoDeserializado.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nombre,
                Selected = m.Id == marcaseleccionada
            }).ToList();
        }

        private async Task<List<Modelo>> ObtenerModelos(Guid marcaId)
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerModelos");

            Console.WriteLine($"URL completa: {endpoint}");
            System.Diagnostics.Debug.WriteLine($"URL completa: {endpoint}");

            using var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, marcaId));

            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();
            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Modelo>>(resultado, opciones) ?? new List<Modelo>();
            }

            return new List<Modelo>();
        }

        public async Task<JsonResult> OnGetObtenerModelos(Guid marcaId)
        {
            var modelos = await ObtenerModelos(marcaId);
            return new JsonResult(modelos);
        }
    }
}
