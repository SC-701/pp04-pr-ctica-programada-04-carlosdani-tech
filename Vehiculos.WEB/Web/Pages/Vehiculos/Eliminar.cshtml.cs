using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    public class EliminarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public EliminarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public VehiculoResponse vehiculo { get; set; } = new();

        public async Task<IActionResult> OnGet(Guid? id)
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerVehiculo");

            Console.WriteLine($"URL completa: {endpoint}");
            System.Diagnostics.Debug.WriteLine($"URL completa: {endpoint}");

            if (!id.HasValue || id == Guid.Empty)
                return NotFound();

            using var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();
            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            vehiculo = JsonSerializer.Deserialize<VehiculoResponse>(resultado, opciones) ?? new VehiculoResponse();
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid? id)
        {
            if (!id.HasValue || id == Guid.Empty)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EliminarVehiculo");

            Console.WriteLine($"URL completa: {endpoint}");
            System.Diagnostics.Debug.WriteLine($"URL completa: {endpoint}");

            using var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Delete, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();
            return RedirectToPage("./Index");
        }
    }
}
