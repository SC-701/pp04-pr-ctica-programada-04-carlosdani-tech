using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    public class EditarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public EditarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        [BindProperty]
        public VehiculoResponse vehiculoResponse { get; set; } = new();

        [BindProperty]
        public List<SelectListItem> marcas { get; set; } = new();

        [BindProperty]
        public List<SelectListItem> modelos { get; set; } = new();

        [BindProperty]
        public Guid marcaseleccionada { get; set; }

        [BindProperty]
        public Guid modeloseleccionado { get; set; }

        public async Task<IActionResult> OnGet(Guid? id)
        {
            if (!id.HasValue || id == Guid.Empty)
                return NotFound();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerVehiculo");

            Console.WriteLine($"URL completa: {endpoint}");
            System.Diagnostics.Debug.WriteLine($"URL completa: {endpoint}");

            using var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();

            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                await ObtenerMarcas();
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                vehiculoResponse = JsonSerializer.Deserialize<VehiculoResponse>(resultado, opciones) ?? new VehiculoResponse();

                var marcaSeleccionada = marcas.FirstOrDefault(m => string.Equals(m.Text, vehiculoResponse.Marca, StringComparison.OrdinalIgnoreCase));
                if (marcaSeleccionada != null)
                {
                    marcaseleccionada = Guid.Parse(marcaSeleccionada.Value);
                }

                if (marcaseleccionada != Guid.Empty)
                {
                    modelos = (await ObtenerModelos(marcaseleccionada)).Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Nombre,
                        Selected = string.Equals(m.Nombre, vehiculoResponse.Modelo, StringComparison.OrdinalIgnoreCase)
                    }).ToList();

                    var modeloSeleccionado = modelos.FirstOrDefault(m => string.Equals(m.Text, vehiculoResponse.Modelo, StringComparison.OrdinalIgnoreCase));
                    if (modeloSeleccionado != null)
                    {
                        modeloseleccionado = Guid.Parse(modeloSeleccionado.Value);
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await CargarFormulario();
                return Page();
            }

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EditarVehiculo");
            using var cliente = new HttpClient();
            var respuesta = await cliente.PutAsJsonAsync(string.Format(endpoint, vehiculoResponse.Id), new VehiculoRequest
            {
                Placa = vehiculoResponse.Placa,
                IdModelo = modeloseleccionado,
                Anio = vehiculoResponse.Anio,
                Color = vehiculoResponse.Color,
                Precio = vehiculoResponse.Precio,
                CorreoPropietario = vehiculoResponse.CorreoPropietario,
                TelefonoPropietario = vehiculoResponse.TelefonoPropietario
            });
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
                    Selected = m.Id == modeloseleccionado
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
