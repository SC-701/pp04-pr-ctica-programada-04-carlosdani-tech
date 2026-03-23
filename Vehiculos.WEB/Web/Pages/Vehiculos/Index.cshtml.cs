using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Web.Pages.Vehiculos
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguracion _configuracion;
        public IList<VehiculoResponse> vehiculos { get; set; } = new List<VehiculoResponse>();
        public IndexModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task OnGet()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints",
                "ObtenerVehiculos");



            Console.WriteLine($"URL completa: {endpoint}");
            System.Diagnostics.Debug.WriteLine($"URL completa: {endpoint}");

            using var cliente = new HttpClient();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();
            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions
            { PropertyNameCaseInsensitive = true };
            vehiculos = JsonSerializer.Deserialize<List<VehiculoResponse>>
                (resultado, opciones) ?? new List<VehiculoResponse>();
        }
    }
}
