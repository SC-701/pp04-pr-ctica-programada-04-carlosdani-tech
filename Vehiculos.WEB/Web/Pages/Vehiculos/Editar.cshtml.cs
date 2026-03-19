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

        [BindProperty]
        public VehiculoDetalle vehiculo { get; set; } = default!;

        [BindProperty]
        public List<SelectListItem> marcas { get; set; } = new();

        [BindProperty]
        public List<SelectListItem> modelos { get; set; } = new();

        [BindProperty]
        public Guid marcaSeleccionada { get; set; }

        [BindProperty]
        public Guid modeloSeleccionado { get; set; }

        public EditarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<ActionResult> OnGet(Guid? id)
        {
            if (!id.HasValue || id == Guid.Empty)
                return NotFound();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerVehiculo");
            var cliente = new HttpClient();

            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);
            if (respuesta.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            respuesta.EnsureSuccessStatusCode();
            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            vehiculo = JsonSerializer.Deserialize<VehiculoDetalle>(resultado, opciones);

            await ObtenerMarcasAsync();

            var marcaActual = marcas.FirstOrDefault(m => m.Text == vehiculo.Marca);
            if (marcaActual != null && Guid.TryParse(marcaActual.Value, out var idMarca))
            {
                marcaSeleccionada = idMarca;
                var modelosDisponibles = await ObtenerModelosAsync(marcaSeleccionada);
                modelos = CrearOpcionesModelos(modelosDisponibles, vehiculo.Modelo);

                var modeloActual = modelos.FirstOrDefault(m => m.Selected);
                if (modeloActual != null && Guid.TryParse(modeloActual.Value, out var idModelo))
                {
                    modeloSeleccionado = idModelo;
                }
            }

            return Page();
        }

        public async Task<ActionResult> OnPost()
        {
            if (vehiculo.Id == Guid.Empty)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await ObtenerMarcasAsync();
                if (marcaSeleccionada != Guid.Empty)
                {
                    modelos = CrearOpcionesModelos(await ObtenerModelosAsync(marcaSeleccionada), modeloSeleccionado);
                }

                return Page();
            }

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EditarVehiculo");
            var cliente = new HttpClient();

            var respuesta = await cliente.PutAsJsonAsync(string.Format(endpoint, vehiculo.Id), new VehiculoRequest
            {
                IdModelo = modeloSeleccionado,
                Anio = vehiculo.Anio,
                Color = vehiculo.Color,
                CorreoPropietario = vehiculo.CorreoPropietario,
                Placa = vehiculo.Placa,
                Precio = vehiculo.Precio,
                TelefonoPropietario = vehiculo.TelefonoPropietario
            });

            respuesta.EnsureSuccessStatusCode();
            return RedirectToPage("./Index");
        }

        private async Task ObtenerMarcasAsync()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerMarcas");
            var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();
            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultadoDeserializado = JsonSerializer.Deserialize<List<Marca>>(resultado, opciones) ?? new List<Marca>();
                marcas = resultadoDeserializado.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id.ToString(),
                                      Text = a.Nombre,
                                      Selected = a.Id == marcaSeleccionada
                                  }).ToList();
            }
        }

        public async Task<JsonResult> OnGetObtenerModelos(Guid marcaId)
        {
            var modelosResultado = await ObtenerModelosAsync(marcaId);
            return new JsonResult(modelosResultado);
        }

        private async Task<List<Modelo>> ObtenerModelosAsync(Guid marcaId)
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerModelos");
            var cliente = new HttpClient();
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

        private static List<SelectListItem> CrearOpcionesModelos(IEnumerable<Modelo> modelos, string? nombreSeleccionado)
        {
            return modelos.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Nombre,
                Selected = a.Nombre == nombreSeleccionado
            }).ToList();
        }

        private static List<SelectListItem> CrearOpcionesModelos(IEnumerable<Modelo> modelos, Guid modeloSeleccionado)
        {
            return modelos.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Nombre,
                Selected = a.Id == modeloSeleccionado
            }).ToList();
        }
    }
}
