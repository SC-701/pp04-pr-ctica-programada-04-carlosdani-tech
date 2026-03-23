using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    public class DetalleModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public VehiculoDetalle vehiculo { get; set; } = new();

        public DetalleModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

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
            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            vehiculo = JsonSerializer.Deserialize<VehiculoDetalle>(resultado, opciones) ?? new VehiculoDetalle();
            return Page();
        }
    }
}
